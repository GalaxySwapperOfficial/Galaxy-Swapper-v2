using Galaxy_Swapper_v2.Workspace.CProvider.Encryption;
using Galaxy_Swapper_v2.Workspace.CProvider.Objects;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Shapes;

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

            Log.Information($"Mounting at {PakDirectoryInfo.FullName}");

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
            path = FormatPath(path).ToLower();

            foreach (var reader in IoStoreReaders)
            {
                foreach (var gameFile in reader.Files)
                {
                    if (gameFile.Key.ToLower().Contains(path))
                    {
                        Log.Information($"Exporting {path}");

                        if (!reader.Export(gameFile.Value, path, (long)gameFile.Value.ChunkOffsetLengths.Offset))
                        {
                            Log.Error($"Failed to export {path}");
                            return null!;
                        }

                        Log.Information($"Successfully exported {path}");
                        return gameFile.Value;
                    }
                }
            }

            Log.Error($"Failed to locate a reader with {path}");
            return null!;
        }

        public List<string> ListFiles()
        {
            var list = new List<string>();

            foreach (var reader in IoStoreReaders)
            {
                list.AddRange(reader.Files.Keys);
            }

            return list;
        }

        public string FindGameFile(string path)
        {
            foreach (var reader in IoStoreReaders)
            {
                string formatted = reader.Files.Keys.FirstOrDefault(gamepath => gamepath.SubstringBeforeLast('.').ToLower().EndsWith(path))!;

                if (string.IsNullOrEmpty(formatted))
                    continue;

                return formatted;
            }

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
            else if (root.Equals("epicbasetextures"))
            {
                ret = string.Concat("fortnitegame", $"/plugins/contentlibraries/{root}/content/", tree);
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