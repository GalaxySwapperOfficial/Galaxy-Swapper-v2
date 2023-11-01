using Galaxy_Swapper_v2.Workspace.CProvider.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Galaxy_Swapper_v2.Workspace.CProvider
{
    public class DefaultFileProvider
    {
        public Dictionary<FGuid, FAesKey> Keys = null!;
        public DirectoryInfo PakDirectoryInfo = null!;
        public List<IoStoreReader> IoStoreReaders = null!;
        public DefaultFileProvider(DirectoryInfo pakDirectoryInfo)
        {
            PakDirectoryInfo = pakDirectoryInfo;
            IoStoreReaders = new();
            Keys = new();
        }

        public void Initialize(List<string> blacklisted = null!)
        {
            var stopwatch = new Stopwatch(); stopwatch.Start();
            int failed = 0;

            foreach (var ioFileInfo in PakDirectoryInfo.GetFiles("*.backup*", SearchOption.TopDirectoryOnly))
            {
                if (!ioFileInfo.Exists || ioFileInfo.Name.ToUpper() == "GLOBAL.BACKUP" || blacklisted is not null && blacklisted.Contains(ioFileInfo.Name.SubstringBefore('.'))) continue;

                var ioStoreReader = new IoStoreReader(this, ioFileInfo, PakDirectoryInfo);

                if (!ioStoreReader.Initialize())
                {
                    Log.Warning($"Failed to mount {ioFileInfo.Name} and will be skipped");
                    failed++; continue;
                }

                IoStoreReaders.Add(ioStoreReader);
            }

            Log.Information($"Mounted {IoStoreReaders.Count} successfully and failed to mount {failed} in {stopwatch.GetElaspedAndStop().ToString("mm':'ss")}");
        }

        public void Initialize(List<string> pakchunks, bool specific)
        {
            var stopwatch = new Stopwatch(); stopwatch.Start();
            int failed = 0;

            foreach (var ioFileInfo in PakDirectoryInfo.GetFiles("*.backup*", SearchOption.TopDirectoryOnly))
            {
                if (!ioFileInfo.Exists || !pakchunks.Contains(ioFileInfo.Name.SubstringBefore('.'))) continue;

                var ioStoreReader = new IoStoreReader(this, ioFileInfo, PakDirectoryInfo);

                if (!ioStoreReader.Initialize())
                {
                    Log.Warning($"Failed to mount {ioFileInfo.Name} and will be skipped");
                    failed++; continue;
                }

                IoStoreReaders.Add(ioStoreReader);
            }

            Log.Information($"Mounted {IoStoreReaders.Count} successfully and failed to mount {failed} in {stopwatch.GetElaspedAndStop().ToString("mm':'ss")}");
        }

        public void SubmitKeys(Dictionary<FGuid, FAesKey> faeskeys)
        {
            if (Keys is null)
                Keys = new();

            foreach (var faeskey in faeskeys)
            {
                if (!Keys.ContainsKey(faeskey.Key))
                {
                    Keys.Add(faeskey.Key, faeskey.Value);
                }
            }
        }

        public GameFile Save(string path)
        {
            path = FormatPath(path);

            foreach (var reader in IoStoreReaders)
            {
                if (reader.Files.ContainsKey(path))
                {
                    var gamefile = reader.Files[path];

                    Log.Information($"Exporting {path}");

                    if (!reader.Export(gamefile, path, (long)gamefile.ChunkOffsetLengths.Offset))
                    {
                        Log.Error($"Failed to export {path}");
                        return null!;
                    }

                    Log.Information($"Successfully exported {path}");
                    return gamefile;
                }
            }

            Log.Error($"Failed to locate a reader with {path}");
            return null!;
        }

        //Modified a bit to match what I want but base Is from: https://github.com/FabianFG/CUE4Parse/blob/master/CUE4Parse/FileProvider/AbstractFileProvider.cs
        private static string FormatPath(string path)
        {
            path = path.Replace('\\', '/');
            path = path.ToLower();

            if (path[0] == '/')
                path = path[1..];

            var ret = path;
            var root = path.SubstringBefore('/');
            var tree = path.SubstringAfter('/');

            if (root.Equals("game") || root.Equals("engine"))
            {
                var gameName = root.Equals("engine") ? "engine" : "fortnitegame";
                var root2 = tree.SubstringBefore('/');

                if (root2.Equals("config") || root2.Equals("content") || root2.Equals("plugins"))
                {
                    ret = string.Concat(gameName, '/', tree);
                }
                else
                {
                    ret = string.Concat(gameName, "/content/", tree);
                }
            }
            else if (root.Equals("fortnitegame"))
            {
                // everything should be good
            }
            else
            {
                ret = string.Concat("fortnitegame", $"/plugins/gamefeatures/{root}/content/", tree);
            }

            if (ret.Contains('.') && ret.EndsWith(".uasset"))
                return ret.ToLower();
            else if (ret.Contains('.'))
                return $"{ret.SubstringBefore('.').ToLower()}.uasset";
            else
                return $"{ret.ToLower()}.uasset";
        }

        public void Dispose()
        {
            foreach (var reader in IoStoreReaders)
            {
                reader.Files.Clear();
                reader.Partitions.Clear();
            }

            Keys.Clear();
            IoStoreReaders.Clear();
            GC.Collect();
        }
    }
}