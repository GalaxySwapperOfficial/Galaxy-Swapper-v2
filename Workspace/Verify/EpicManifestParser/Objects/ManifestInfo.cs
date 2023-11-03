using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Galaxy_Swapper_v2.Workspace.Verify.EpicManifestParser.Objects
{
    public class ManifestInfo : DownloadableManifestInfo
    {
        public string AppName { get; }
        public string LabelName { get; }
        public string BuildVersion { get; set; }
        public Version Version { get; }
        public int CL { get; }
        public string Hash { get; }

        public ManifestInfo() { }
        public ManifestInfo(Stream jsonStream, int idx = 0) : this(JsonDocument.Parse(jsonStream), idx) { }
        public ManifestInfo(byte[] jsonBytes, int idx = 0) : this(JsonDocument.Parse(jsonBytes), idx) { }
        public ManifestInfo(string jsonString, int idx = 0) : this(JsonDocument.Parse(jsonString), idx) { }

        public ManifestInfo(JsonDocument jsonDocument, int idx = 0)
        {
            var rootElement = jsonDocument.RootElement.GetProperty("elements")[idx];
            AppName = rootElement.GetProperty("appName").GetString();
            LabelName = rootElement.GetProperty("labelName").GetString();
            BuildVersion = rootElement.GetProperty("buildVersion").GetString();
            Hash = rootElement.GetProperty("hash").GetString();

            var buildMatch = Regex.Match(BuildVersion, @"(\d+(?:\.\d+)+)-CL-(\d+)", RegexOptions.Singleline);
            if (buildMatch.Success)
            {
                Version = Version.Parse(buildMatch.Groups[1].Value);
                CL = int.Parse(buildMatch.Groups[2].Value);
            }

            var manifestUriBuilders = new List<UriBuilder>();
            var manifestsArray = rootElement.GetProperty("manifests");
            foreach (var manifestElement in manifestsArray.EnumerateArray())
            {
                var uriBuilder = new UriBuilder(manifestElement.GetProperty("uri").GetString());

                if (manifestElement.TryGetProperty("queryParams", out var queryParamsArray))
                {
                    foreach (var query in from queryParam in queryParamsArray.EnumerateArray()
                                          let queryParamName = queryParam.GetProperty("name").GetString()
                                          let queryParamValue = queryParam.GetProperty("value").GetString()
                                          select $"{queryParamName}={queryParamValue}")
                    {
                        if (uriBuilder.Query.Length == 0)
                        {
                            uriBuilder.Query = query;
                        }
                        else
                        {
                            uriBuilder.Query += '&' + query;
                        }
                    }
                }

                manifestUriBuilders.Add(uriBuilder);
            }

            Uris = manifestUriBuilders;
            FileName = Uris.FirstOrDefault()?.Uri.Segments[^1];
        }
    }
}