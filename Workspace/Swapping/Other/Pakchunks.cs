using CUE4Parse.UE4.IO.Objects;
using CUE4Parse.Utils;
using Galaxy_Swapper_v2.Workspace.Properties;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Serilog;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Galaxy_Swapper_v2.Workspace.Swapping.Other
{
    public static class Pakchunks
    {
        public static void Validate(string path)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var directoryInfo = new DirectoryInfo(path);

            Log.Information("Checking if game files are in use");
            foreach (var file in directoryInfo.EnumerateFiles("*.*", SearchOption.TopDirectoryOnly))
            {
                if ((File.GetAttributes(file.FullName) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    File.SetAttributes(file.FullName, File.GetAttributes(file.FullName) & ~FileAttributes.ReadOnly);
                }
                if (!file.FullName.CanEdit())
                {
                    Log.Error(file.Name + " is in use!");
                    throw new Global.CustomException("Fortnite game files are currently in use!\nPlease close anything that may be using your game files.");
                }
            }

            Log.Information("Checking for unused backup game files");
            foreach (var file in directoryInfo.EnumerateFiles("*.backup*", SearchOption.TopDirectoryOnly))
            {
                var orignal = new FileInfo(file.FullName.Replace(file.Extension, ".utoc"));
                if (!orignal.Exists)
                {
                    Log.Information($"{file.Name} exists but orignal {orignal.Name} does not backup will be removed");
                    File.Delete(file.FullName);
                }
            }

            var parse = Endpoint.Read(Endpoint.Type.UEFN);
            string[] uefnSlots = parse["Slots"].ToObject<string[]>();
            byte[] mountPoint = Encoding.ASCII.GetBytes("/FortniteGame/Plugins/GameFeatures/");

            foreach (string slot in uefnSlots)
            {
                var fileInfo = new FileInfo($"{directoryInfo.FullName}\\{slot}.pak");
                if (fileInfo.Exists)
                {
                    Log.Information($"Checking if {fileInfo.Name} is not UEFN");
                    using (FileStream stream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.ReadWrite))
                    {
                        long position = Misc.IndexOfSequence(stream, mountPoint);

                        stream.Close(); //Don't need it.

                        if (position < 0)
                        {
                            Log.Information($"{fileInfo.Name} is not uefn and will be removed");
                            UEFN.Delete($"{directoryInfo.FullName}\\{slot}.ucas");
                            UEFN.Delete($"{directoryInfo.FullName}\\{slot}.utoc");
                            UEFN.Delete($"{directoryInfo.FullName}\\{slot}.pak");
                            UEFN.Delete($"{directoryInfo.FullName}\\{slot}.sig");
                            UEFN.Delete($"{directoryInfo.FullName}\\{slot}.backup");
                        }
                    }
                }
            }

            Log.Information("Backing up game files");
            foreach (var file in directoryInfo.EnumerateFiles("*.backup*", SearchOption.TopDirectoryOnly))
            {
                FileInfo fileInfo = new FileInfo(file.FullName.SubstringBeforeLast('.') + ".backup");

                if (fileInfo.Exists)
                {
                    var reader = new Reader(file.FullName);
                    var readerbackup = new Reader(fileInfo.FullName);

                    var header = new FIoStoreTocHeader(reader);
                    var headerbackup = new FIoStoreTocHeader(readerbackup);

                    if (header.Compare(headerbackup) && reader.Length == readerbackup.Length)
                    {
                        reader.Dispose();
                        readerbackup.Dispose();
                        continue;
                    }

                    reader.Dispose();
                    readerbackup.Dispose();

                    Log.Warning($"{fileInfo.Name} IO header does not match {file.Name} attempting to remove backup");
                    File.Delete(fileInfo.FullName);
                }

                Log.Information($"{fileInfo.Name} does not exist!");
                Copy(directoryInfo, file, fileInfo.FullName);
            }

            stopwatch.Stop();
            Log.Information($"Finished backing up game files in {stopwatch.Elapsed.TotalSeconds} seconds!");
        }

        public static void Backup(string path)
        {

        }

        private static void Copy(DirectoryInfo directoryInfo, FileInfo ioinfo, string dest, bool overwrite = false)
        {
            long availableFreeSpace = new DriveInfo(directoryInfo.Root.Name).AvailableFreeSpace;
            if (availableFreeSpace < ioinfo.Length)
            {
                Log.Error($"{directoryInfo.Root.Name} is out of space!\nNeeded: {ioinfo.Length}\nHas: {availableFreeSpace}");
                throw new Global.CustomException($"{directoryInfo.Root.Name} does not have enough space to make backup!\nNeeded: {ioinfo.Length}\nHas: {availableFreeSpace}\nPlease make room on your drive in order to backup!");
            }
            Log.Information("Copying " + ioinfo.FullName + " to " + dest);
            File.Copy(ioinfo.FullName, dest, overwrite);
        }
    }
}