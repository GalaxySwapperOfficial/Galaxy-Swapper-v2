using CUE4Parse.UE4.IO.Objects;
using CUE4Parse.Utils;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Serilog;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Galaxy_Swapper_v2.Workspace.Swapping.Other
{
    public static class PaksCheck
    {
        public static void Validate(string path)
        {
            Stopwatch stopwatch = new Stopwatch();
            DirectoryInfo directoryInfo = new DirectoryInfo(path);

            stopwatch.Start();

            Log.Information("Validating game files");

            foreach (FileInfo item in directoryInfo.EnumerateFiles("*.*", SearchOption.TopDirectoryOnly))
            {
                item.Extension.SubstringAfter('.').ToUpper();

                if ((File.GetAttributes(item.FullName) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    Log.Warning($"{item} is marked as readonly!");
                    File.SetAttributes(item.FullName, File.GetAttributes(item.FullName) & ~FileAttributes.ReadOnly);
                }
                if (!item.FullName.CanEdit())
                {
                    Log.Error(item.FullName + " is in use!");
                    throw new Global.CustomException("Fortnite game files are currently in use!\nPlease close anything that may be using your game files.");
                }
            }

            stopwatch.Stop();
            Log.Information($"Validated {new DirectoryInfo(path).GetFiles().Count()} game files in {stopwatch.Elapsed.TotalSeconds} seconds!");
        }

        public static void Backup(string path)
        {
            Stopwatch stopwatch = new Stopwatch();
            DirectoryInfo directoryInfo = new DirectoryInfo(path);

            stopwatch.Start();

            Log.Information("Starting IO backup task");

            foreach (FileInfo item in directoryInfo.EnumerateFiles("*.utoc*", SearchOption.TopDirectoryOnly))
            {
                FileInfo fileInfo = new FileInfo(item.FullName.SubstringBeforeLast('.') + ".backup");

                if (fileInfo.Exists)
                {
                    var reader = new Reader(item.FullName);
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

                    Log.Warning($"{fileInfo.Name} IO header does not match {item.Name} attempting to remove backup");
                    File.Delete(fileInfo.FullName);
                }

                Log.Information($"{fileInfo.Name} does not exist!");
                Copy(directoryInfo, item, fileInfo.FullName);
            }

            stopwatch.Stop();
            Log.Information($"Finished backing up game files in {stopwatch.Elapsed.TotalSeconds} seconds!");
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