using Galaxy_Swapper_v2.Workspace.Utilities;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

namespace Galaxy_Swapper_v2.Workspace.Properties
{
    public static class DLLS
    {
        public static readonly string Path = $"{Config.Path}\\DLLS";
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool SetDllDirectory(string lpPathName);

        public static void Initialize()
        {
            SetDllDirectory(Path);
            Log.Information($"Set DLL directory to {Path}");

            if (!Create($"{Path}\\{DLLData.OODLE_5}", DLLData.OODLE_5_Buffer, DLLData.OODLE_5_Length))
            {
                Message.Display("Error", "Failed to initialize dlls", MessageBoxButton.OK, new List<string> { "Run Galaxy Swapper v2 as administrator", "Disable any anti virus software", "Ensure you don't have Galaxy Swapper v2 open twice" });
                Environment.Exit(0);
            }
        }

        private static bool Create(string path, string base64, long size)
        {
            if (!File.Exists(path) || path.FileLength() != size) 
            {
                Log.Information($"{path} does not exist attempting to create");

                try
                {
                    byte[] buffertowrite = Compression.Decompress(base64);

                    if (File.Exists(path))
                        File.Delete(path);

                    File.WriteAllBytes(path, buffertowrite);

                    Log.Information($"successfully wrote to {path}");
                }
                catch (Exception Exception)
                {
                    Log.Fatal(Exception, $"Failed to write byte[{size}] to {path}");
                    return false;
                }
            }

            return true;
        }
    }
}