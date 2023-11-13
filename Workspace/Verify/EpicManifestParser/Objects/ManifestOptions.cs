using System;
using System.IO;

namespace Galaxy_Swapper_v2.Workspace.Verify.EpicManifestParser.Objects
{
    public class ManifestOptions
    {
        public Uri ChunkBaseUri { get; set; }
        public DirectoryInfo ChunkCacheDirectory { get; set; }
    }
}