using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Galaxy_Swapper_v2.Workspace.Verify.EpicManifestParser.Objects
{
    public class ContentBuildManifestInfo : DownloadableManifestInfo
    {
        public string AppName { get; }
        public string LabelName { get; }
        public string BuildVersion { get; set; }
        public string CatalogItemId { get; set; }
        public Version Version { get; }
        public int CL { get; }

        public ContentBuildManifestInfo() { }
        public ContentBuildManifestInfo(Stream jsonStream) : this(JsonDocument.Parse(jsonStream)) { }
        public ContentBuildManifestInfo(byte[] jsonBytes) : this(JsonDocument.Parse(jsonBytes)) { }
        public ContentBuildManifestInfo(string jsonString) : this(JsonDocument.Parse(jsonString)) { }

        public ContentBuildManifestInfo(JsonDocument jsonDocument)
        {
            var rootElement = jsonDocument.RootElement;
            AppName = rootElement.GetProperty("appName").GetString();
            LabelName = rootElement.GetProperty("labelName").GetString();
            BuildVersion = rootElement.GetProperty("buildVersion").GetString();
            CatalogItemId = rootElement.GetProperty("catalogItemId").GetString();

            var buildMatch = Regex.Match(BuildVersion, @"(\d+(?:\.\d+)+)-CL-(\d+)", RegexOptions.Singleline);
            if (buildMatch.Success)
            {
                Version = Version.Parse(buildMatch.Groups[1].Value);
                CL = int.Parse(buildMatch.Groups[2].Value);
            }

            var manifestUriBuilders = new List<UriBuilder>();
            var manifest = rootElement.GetProperty("items").GetProperty("MANIFEST");

            var distribution = manifest.GetProperty("distribution").GetString();
            var path = manifest.GetProperty("path").GetString();
            var query = manifest.GetProperty("signature").GetString();
            manifestUriBuilders.Add(new UriBuilder(distribution + path));

            var additionalDistributions = manifest.GetProperty("additionalDistributions").EnumerateArray();
            foreach (var additionalDistrib in additionalDistributions)
            {
                manifestUriBuilders.Add(new UriBuilder(additionalDistrib.GetString() + path));
            }

            foreach (var manifestUri in manifestUriBuilders)
            {
                if (manifestUri.Query.Length == 0)
                {
                    manifestUri.Query = query;
                }
                else
                {
                    manifestUri.Query += '&' + query;
                }
            }

            Uris = manifestUriBuilders;
            FileName = path[(path.LastIndexOf("/", StringComparison.Ordinal) + 1)..];
        }
    }
}