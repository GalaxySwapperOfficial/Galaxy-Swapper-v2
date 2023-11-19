using Galaxy_Swapper_v2.Workspace.CProvider;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;
using Serilog;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static Galaxy_Swapper_v2.Workspace.Global;

namespace Galaxy_Swapper_v2.Workspace.Swapping.Other
{
    public static class Pakchunks
    {
        public static void Validate(string path)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var directoryInfo = new DirectoryInfo(path);

            if (!directoryInfo.Exists)
            {
                Log.Error($"{directoryInfo.FullName} does not exist?");
                throw new CustomException($"{directoryInfo.FullName}\nDoes not exist? Try selecting your Fortnite location in settings.");
            }

            if (!File.Exists($"{directoryInfo.FullName}\\global.utoc") || !File.Exists($"{directoryInfo.FullName}\\pakchunk10-WindowsClient.utoc") || !File.Exists($"{directoryInfo.FullName}\\pakchunk20-WindowsClient.utoc"))
            {
                Log.Error($"{directoryInfo.FullName} is missing core game files logging all available game files for debugging");
                directoryInfo.GetFiles().ToList().ForEach(fileInfo => Log.Information(fileInfo.Name));
                throw new CustomException($"{directoryInfo.FullName}\nIs missing core game files that are requried to mount correctly.");
            }

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
                    throw new CustomException("Fortnite game files are currently in use!\nPlease close anything that may be using your game files.");
                }
            }

            var parse = Endpoint.Read(Endpoint.Type.UEFN);

            if (parse["Enabled"].Value<bool>())
            {
                Log.Information("Validating UEFN slots");

                string[] uefnSlots = parse["Slots"].ToObject<string[]>();
                byte[] mountpoint = new byte[] { 47, 70, 111, 114, 116, 110, 105, 116, 101, 71, 97, 109, 101, 47, 80, 108, 117, 103, 105, 110, 115, 47 };

                foreach (string slot in uefnSlots)
                {
                    var pakInfo = new FileInfo($"{directoryInfo.FullName}\\{slot}.pak");

                    if (pakInfo.Exists)
                    {
                        Log.Information($"Validating {pakInfo.Name}");

                        using (FileStream pakStream = new FileStream(pakInfo.FullName, FileMode.Open, FileAccess.Read))
                        {
                            long position = Misc.IndexOfSequence(pakStream, mountpoint);

                            pakStream.Close();

                            if (position < 0)
                            {
                                Log.Information($"{pakInfo.Name} is not uefn and will be removed");

                                Misc.TryDelete($"{directoryInfo.FullName}\\{slot}.ucas");
                                Misc.TryDelete($"{directoryInfo.FullName}\\{slot}.utoc");
                                Misc.TryDelete($"{directoryInfo.FullName}\\{slot}.pak");
                                Misc.TryDelete($"{directoryInfo.FullName}\\{slot}.sig");
                                Misc.TryDelete($"{directoryInfo.FullName}\\{slot}.backup");
                            }
                        }
                    }
                }
            }

            Log.Information("Checking for unused backup game files");
            foreach (var backupIoFileInfo in directoryInfo.EnumerateFiles("*.backup*", SearchOption.TopDirectoryOnly))
            {
                var ioFileInfo = new FileInfo($"{backupIoFileInfo.FullName.SubstringBeforeLast('.')}.utoc");
                if (!ioFileInfo.Exists)
                {
                    Log.Information($"{backupIoFileInfo.Name} exists but orignal {ioFileInfo.Name} does not backup will be removed");
                    File.Delete(backupIoFileInfo.FullName);
                }
            }

            Log.Information("Checking for outdated backup game files");
            foreach (var backupIoFileInfo in directoryInfo.EnumerateFiles("*.backup*", SearchOption.TopDirectoryOnly))
            {
                var ioFileInfo = new FileInfo($"{backupIoFileInfo.FullName.SubstringBeforeLast('.')}.utoc");

                //This should never happen but just incase.
                if (!ioFileInfo.Exists || !backupIoFileInfo.Exists)
                    continue;

                var ioReader = new Reader(ioFileInfo.FullName);
                var backupIoReader = new Reader(backupIoFileInfo.FullName);

                byte[] Magic = { 0x2D, 0x3D, 0x3D, 0x2D, 0x2D, 0x3D, 0x3D, 0x2D, 0x2D, 0x3D, 0x3D, 0x2D, 0x2D, 0x3D, 0x3D, 0x2D };

                if (backupIoReader.ReadBytes(Magic.Length).SequenceEqual(Magic))
                {
                    backupIoReader.Position = 0;

                    var ioHeader = new FIoStoreTocHeader(ioReader);
                    var backupIoHeader = new FIoStoreTocHeader(backupIoReader);

                    if (ioHeader.Compare(backupIoHeader) && ioReader.Length == backupIoReader.Length)
                    {
                        ioReader.Close();
                        backupIoReader.Close();

                        continue;
                    }
                    else Log.Warning($"IO backup {backupIoFileInfo.Name} header does not match IO {ioFileInfo.Name} header and will be removed");
                }
                else Log.Warning($"IO backup {backupIoFileInfo.Name} has invalid magic and will be removed");


                ioReader.Close();
                backupIoReader.Close();

                if (!Misc.TryDelete(backupIoFileInfo.FullName))
                {
                    throw new CustomException($"Failed to remove outdated IO backup file: {backupIoFileInfo.FullName}");
                }
            }

            Log.Information("Backing up game files");
            foreach (var ioFileInfo in directoryInfo.EnumerateFiles("*.utoc*", SearchOption.TopDirectoryOnly))
            {
                var backupIoFileInfo = new FileInfo($"{ioFileInfo.FullName.SubstringBeforeLast('.')}.backup");
                if (!backupIoFileInfo.Exists)
                {
                    Copy(directoryInfo, ioFileInfo, backupIoFileInfo.FullName);
                }
            }

            if (!File.Exists($"{directoryInfo.FullName}\\global.backup") || !File.Exists($"{directoryInfo.FullName}\\pakchunk10-WindowsClient.backup") || !File.Exists($"{directoryInfo.FullName}\\pakchunk20-WindowsClient.backup"))
            {
                Log.Error($"{directoryInfo.FullName} io backup task did not backup core game files logging all available game files for debugging");
                directoryInfo.GetFiles().ToList().ForEach(fileInfo => Log.Information(fileInfo.Name));
                throw new CustomException($"{directoryInfo.FullName}\nIO backup task did not backup core game files?");
            }

            Log.Information($"Finished validating game files in {stopwatch.GetElaspedAndStop().TotalSeconds} seconds!");
        }

        private static void Copy(DirectoryInfo directoryInfo, FileInfo orignalFileInfo, string dest, bool delete = false, bool overwrite = false)
        {
            if (delete)
            {
                Log.Information($"Deleting {dest}");

                if (!Misc.TryDelete(dest))
                {
                    throw new CustomException($"Failed to remove {dest} to make room for backup");
                }
            }

            long availableFreeSpace = new DriveInfo(directoryInfo.Root.Name).AvailableFreeSpace;
            if (availableFreeSpace < orignalFileInfo.Length)
            {
                Log.Error($"{directoryInfo.Root.Name} is out of space!\nNeeded: {orignalFileInfo.Length}\nHas: {availableFreeSpace}");
                throw new CustomException($"{directoryInfo.Root.Name} does not have enough space to make backup!\nNeeded: {orignalFileInfo.Length}\nHas: {availableFreeSpace}\nPlease make room on your drive in order to backup!");
            }

            Log.Information("Copying " + orignalFileInfo.FullName + " to " + dest);
            File.Copy(orignalFileInfo.FullName, dest, overwrite);
        }
    }
}