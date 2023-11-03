using System;

namespace Galaxy_Swapper_v2.Workspace.Verify.EpicManifestParser.Objects
{
    public class FileChunk
    {
        public string Guid { get; }
        public long Size { get; }
        public string Hash { get; }
        public string Sha { get; }
        public byte DataGroup { get; }
        public string Filename { get; }
        public Uri Uri { get; }

        internal FileChunk(string guid, long size, string hash, string sha, byte dataGroup, Uri baseUri = null)
        {
            Guid = guid;
            Size = size;
            Hash = hash;
            Sha = sha;
            DataGroup = dataGroup;
            Filename = $"{hash}_{guid}.chunk";
            Uri = baseUri == null ? null : new Uri(baseUri, $"{dataGroup:D2}/{Filename}");
        }
    }
}