using Serilog;
using System;
using System.IO;

namespace Galaxy_Swapper_v2.Workspace.Properties
{
    public static class Output
    {
        public static readonly string Path = $"{App.Config}\\Logs";
        public static string Current = $"{Path}\\Galaxy-Swapper.log";
        private const string Template = "[{Timestamp:M/d/yyyy h:mm tt}] [{Level:u3}] {Message:lj}{NewLine}{Exception}";

        private const int MaxLogFilesToDelete = 10;
        private const int MaxFallbackAttempts = 500;
        public static void Initialize()
        {
            if (Directory.GetFiles(Path).Length > MaxLogFilesToDelete)
            {
                try
                {
                    Directory.Delete(Path, true);
                }
                catch
                {
                    //Ignore this error not a big deal.
                }
            }

            if (File.Exists(Current))
            {
                try
                {
                    var fileinfo = new FileInfo(Current);
                    File.Move(Current, $"{Path}\\Galaxy-Swapper-backup-{fileinfo.CreationTime.ToString("yyyy.MM.dd.hh.mm")}.log", true);
                }
                catch
                {
                    Current = FindOpenFallBackSlot();
                }
            }

            Log.Logger = new LoggerConfiguration().WriteTo.File(Current, outputTemplate: Template).WriteTo.Debug(outputTemplate: Template).CreateLogger();
            Log.Information($"Successfully created log file at {DateTime.Now.ToString("M/d/yyyy h:mm tt")}");
        }

        public static string FindOpenFallBackSlot()
        {
            int count = 0;
            while (count < MaxFallbackAttempts)
            {
                string path = $"{Path}\\Galaxy-Swapper-{count}.log";
                if (!File.Exists(path))
                {
                    return path;
                }

                count++;
            }

            throw new Exception("Can not find open fallback slot for log file");
        }
    }
}