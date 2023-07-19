using Serilog;
using System;
using System.IO;
using System.Linq;

namespace Galaxy_Swapper_v2.Workspace.Properties
{
    /// <summary>
    /// All the code below was provided from: https://github.com/GalaxySwapperOfficial/Galaxy-Swapper-v2
    /// You can also find us at https://galaxyswapperv2.com/Guilded
    /// </summary>
    public static class Output
    {
        public static readonly string Path = $"{Config.Path}\\Logs";
        public static void Initialize()
        {
            var DirectoryInfo = new DirectoryInfo(Path);
            if (DirectoryInfo.GetFiles().Count() > 10)
            {
                foreach (var LogFile in DirectoryInfo.GetFiles())
                {
                    try
                    {
                        File.Delete(LogFile.FullName);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            var CurrentDate = DateTime.Now;
            string Name = $"{Path}\\Galaxy-Swapper-{CurrentDate.ToString("yyyy.MM.dd.hh.mm")}.log";

            Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.File(Name, rollingInterval: RollingInterval.Infinite, retainedFileCountLimit: null, fileSizeLimitBytes: null, outputTemplate: "[{Level:u3}] LOG : {Message:lj}{NewLine}{Exception}").CreateLogger();
            Log.Information($"Successfully created log file at {CurrentDate.ToString("yyyy/MM/dd/ hh:mm")}");
        }
    }
}