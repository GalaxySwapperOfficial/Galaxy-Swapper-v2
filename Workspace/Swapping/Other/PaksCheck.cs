using Galaxy_Swapper_v2.Workspace.Utilities;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Galaxy_Swapper_v2.Workspace.Swapping.Other
{
    /// <summary>
    /// All the code below was provided from: https://github.com/GalaxySwapperOfficial/Galaxy-Swapper-v2
    /// You can also find us at https://galaxyswapperv2.com/Guilded
    /// </summary>
    public static class PaksCheck
    {
        public static bool Run(string path)
        {
            var Stopwatch = new Stopwatch();
            Stopwatch.Start();

            bool isinuse = false;

            foreach (string file in Directory.EnumerateFiles(path))
            {
                try
                {
                    FileAttributes attributes = File.GetAttributes(file);

                    if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        Log.Information($"{file} is marked as readonly!");
                        File.SetAttributes(file, File.GetAttributes(file) & ~FileAttributes.ReadOnly);
                    }
                    if (!file.CanEdit())
                    {
                        Log.Information($"{file} is in use!");
                        isinuse = true;
                    }
                }
                catch (Exception Exception)
                {
                    Log.Fatal(Exception, "Failed to validate game files");
                    throw;
                }
            }

            Log.Information($"Validated {new DirectoryInfo(path).GetFiles().Count()} game files in {Stopwatch.Elapsed.TotalSeconds} seconds");
            return isinuse;
        }
    }
}