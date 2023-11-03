using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy_Swapper_v2.Workspace.Verify.EpicManifestParser.Objects
{
    public abstract class DownloadableManifestInfo
    {
        public List<UriBuilder> Uris { get; protected set; }
        public string FileName { get; protected set; }

        public byte[] DownloadManifestData(string cacheDir = null)
        {
            var path = cacheDir == null ? null : Path.Combine(cacheDir, FileName);
            if (path != null && File.Exists(path))
            {
                return File.ReadAllBytes(path);
            }

            using var client = new HttpClient();
            byte[] data = null;

            foreach (var uri in Uris.TakeWhile(_ => data == null))
            {
                try
                {
                    data = client.GetByteArrayAsync(uri.Uri).GetAwaiter().GetResult();
                }
                catch
                {
                    data = null;
                }
            }

            if (path != null && data != null)
            {
                File.WriteAllBytes(path, data);
            }

            return data;
        }

        public string DownloadManifestString(string cacheDir = null)
        {
            return Encoding.UTF8.GetString(DownloadManifestData(cacheDir));
        }

        public async Task<byte[]> DownloadManifestDataAsync(string cacheDir = null)
        {
            var path = cacheDir == null ? null : Path.Combine(cacheDir, FileName);
            if (path != null && File.Exists(path))
            {
                return await File.ReadAllBytesAsync(path).ConfigureAwait(false);
            }

            using var client = new HttpClient();
            byte[] data = null;

            foreach (var uri in Uris.TakeWhile(_ => data == null))
            {
                try
                {
                    data = await client.GetByteArrayAsync(uri.Uri).ConfigureAwait(false);
                }
                catch
                {
                    data = null;
                }
            }

            if (path != null && data != null)
            {
                await File.WriteAllBytesAsync(path, data).ConfigureAwait(false);
            }

            return data;
        }

        public async Task<string> DownloadManifestStringAsync(string cacheDir = null)
        {
            var data = await DownloadManifestDataAsync(cacheDir).ConfigureAwait(false);
            return Encoding.UTF8.GetString(data);
        }
    }
}