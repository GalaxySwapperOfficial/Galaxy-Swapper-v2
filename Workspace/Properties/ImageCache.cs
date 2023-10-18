using Serilog;
using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Galaxy_Swapper_v2.Workspace.Properties
{
    public static class ImageCache
    {
        public static readonly string Path = App.Config + "\\Cache";
        public static void Initialize()
        {
            try
            {
                if (!Directory.Exists(Path))
                {
                    Log.Information($"{Path} does not exist and will be created");
                    Directory.CreateDirectory(Path);
                }
                if (!File.Exists($"{Path}\\README.txt")) //wHat aRe bIt fIles?
                {
                    Log.Information($"{Path}\\README.txt does not exist and will be created");
                    File.WriteAllText($"{Path}\\README.txt", "These cache files are images loaded from Galaxy Swapper. To check yourself, simply change the file extension from .cache to .png");
                }

                Log.Information("Successfully initialized cache");
            }
            catch (Exception exception)
            {
                Log.Error(exception.Message, $"Caught a error while initializing cache directory");
            }
        }

        public static void Cache(string name, BitmapImage bitmapImage)
        {
            try
            {
                string path = $"{Path}\\{name}";

                if (!Directory.Exists(Path))
                {
                    Log.Information($"{Path} does not exist and will be created");
                    Directory.CreateDirectory(Path);
                }

                if (File.Exists(path))
                {
                    Log.Information($"{path} already exist!");
                    return;
                }

                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));

                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    encoder.Save(stream);
                }

                Log.Information($"Wrote image cache to: {path}");
            }
            catch (Exception exception)
            {
                Log.Error(exception.Message, $"Failed to write image cache to {Path}");
            }
        }

        public static bool ReadCache(string name, BitmapImage bitmapImage)
        {
            try
            {
                string path = $"{Path}\\{name}";

                if (!Directory.Exists(Path) || !File.Exists(path))
                {
                    return false;
                }

                bitmapImage.BeginInit();
                bitmapImage.UriSource = new(path);
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return true;
            }
            catch (Exception exception)
            {
                Log.Error(exception.Message, $"Failed to read image cache from: {Path}");
                return false;
            }
        }
    }
}
