using Galaxy_Swapper_v2.Workspace.ClientSettings.Objects;
using Galaxy_Swapper_v2.Workspace.Swapping.Other;
using Galaxy_Swapper_v2.Workspace.Utilities;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Galaxy_Swapper_v2.Workspace.ClientSettings
{
    public class ClientSettingsData
    {
        public byte[] Buffer = null!;

        public string AccessToken = null!;
        public string AccountID = null!;

        //Compressed Header
        public uint Magic;
        public int Engine;

        //Decompressed Data
        public byte[] DecompressedMagic = null!;
        public string FortniteVersion = null!;

        public int _reserved0;
        public byte _reserved1;
        public byte _reserved2;

        public FCustomVersion[] CustomVersions = null!;
        public byte[] RestOfData = null!;

        public bool Deserialize()
        {
            Log.Information("Deserializing ClientSettings buffer");

            try
            {
                var reader = new Reader(Buffer);

                Magic = reader.Read<uint>();

                if (!Magic.Equals(0x44464345))
                {
                    throw new Exception("ClientSettings.sav magic is not valid");
                }

                Engine = reader.Read<int>();

                var isCompressed = reader.ReadBoolean(true);

                if (isCompressed)
                {
                    Log.Information("Decompressing ClientSettings buffer");

                    var decompressedSize = reader.Read<int>();
                    var compressedSize = (int)(reader.BaseStream.Length - reader.BaseStream.Position);

                    byte[] compressedBuffer = reader.ReadBytes(compressedSize);
                    byte[] decompressedBuffer = Decompress(compressedBuffer, decompressedSize);

                    reader = new Reader(decompressedBuffer);
                }
                else
                {
                    Log.Warning("ClientSettings is not compressed as expected and may not work correctly");
                }

                DecompressedMagic = reader.ReadBytes(22);
                FortniteVersion = reader.ReadFString();

                _reserved0 = reader.Read<int>();
                _reserved1 = reader.ReadByte();
                _reserved2 = reader.ReadByte();

                var customVersionCount = reader.Read<int>();

                CustomVersions = new FCustomVersion[customVersionCount];
                for (int i = 0; i < customVersionCount; i++)
                {
                    CustomVersions[i] = new FCustomVersion(reader);
                }

                RestOfData = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));

                return true;
            }
            catch (Exception exception)
            {
                Log.Error(exception.Message);
                Message.DisplaySTA("Error", $"Failed to deserialize ClientSettings.Sav buffer!\n\n{exception.Message}", System.Windows.MessageBoxButton.OK, discord: true, solutions: new string[] { "Please restore Fortnite settings to default, then apply them and attempt to swap again." });
                return false;
            }
        }

        public bool Serialize(out byte[] serialized)
        {
            serialized = null!;

            Log.Information("Serializing ClientSettings buffer");

            try
            {
                var writer = new Writer(new byte[RestOfData.Length + 80000]);

                writer.WriteBytes(DecompressedMagic);
                writer.WriteFString(FortniteVersion);

                writer.Write<int>(_reserved0);
                writer.WriteByte(_reserved1);
                writer.WriteByte(_reserved2);

                writer.Write<int>(CustomVersions.Length);

                foreach (var customVersion in CustomVersions)
                {
                    customVersion.Write(writer);
                }

                writer.WriteBytes(RestOfData);

                byte[] serializedBuffer = writer.ToByteArray(writer.Position);
                byte[] compressedSerializedBuffer = Compress(serializedBuffer);

                writer = new Writer(new byte[RestOfData.Length + 50000]);

                writer.Write<uint>(Magic);
                writer.Write<int>(Engine);

                writer.WriteBoolean(true, true);

                writer.Write<int>(serializedBuffer.Length);

                writer.WriteBytes(compressedSerializedBuffer);

                serialized = writer.ToByteArray(writer.Position);

                return true;
            }
            catch (Exception exception)
            {
                Log.Error(exception.Message);
                Message.DisplaySTA("Error", $"Failed to serialize ClientSettings.Sav buffer!\n\n{exception.Message}", System.Windows.MessageBoxButton.OK, discord: true, solutions: new string[] { "Please restore Fortnite settings to default, then apply them and attempt to swap again." });
                return false;
            }
        }

        public bool ModifyFov(float fov)
        {
            try
            {
                Log.Information("Modifying FOVMinimum/FOVMaximum values");

                var minFovSearch = Encoding.ASCII.GetBytes("FOVMinimum");
                var maxFovSearch = Encoding.ASCII.GetBytes("FOVMaximum");

                int minFovPos = RestOfData.IndexOfSequence(minFovSearch, 0);
                int maxFovPos = RestOfData.IndexOfSequence(maxFovSearch, 0);

                if (minFovPos < 0 || maxFovPos < 0)
                {
                    throw new Exception("Failed to find fov position in clientsettings.sav buffer");
                }

                minFovPos += minFovSearch.Length;
                maxFovPos += maxFovSearch.Length;

                var writer = new Writer(RestOfData);

                writer.Position = minFovPos + 29;
                writer.Write<float>(fov - 1);

                writer.Position = maxFovPos + 29;
                writer.Write<float>(fov);

                writer.Position = 0;
                RestOfData = writer.ToByteArray(writer.BaseStream.Length);

                return true;
            }
            catch (Exception exception)
            {
                Log.Error(exception.Message);
                Message.DisplaySTA("Error", $"Failed to modify fov values!\n\n{exception.Message}", System.Windows.MessageBoxButton.OK, discord: true, solutions: new string[] { "Please restore Fortnite settings to default, then apply them and attempt to swap again." });
                return false;
            }
        }

        public bool Authenticate(string authorization_code)
        {
            var stopwatch = new Stopwatch(); stopwatch.Start();
            const string url = "https://account-public-service-prod.ol.epicgames.com/account/api/oauth/token";

            using (var client = new RestClient())
            {
                var request = new RestRequest(new Uri(url), Method.Post);
                request.AddHeader("Authorization", "Basic MzQ0NmNkNzI2OTRjNGE0NDg1ZDgxYjc3YWRiYjIxNDE6OTIwOWQ0YTVlMjVhNDU3ZmI5YjA3NDg5ZDMxM2I0MWE=");

                request.AddParameter("grant_type", "authorization_code");
                request.AddParameter("token_type", "eg1");
                request.AddParameter("code", authorization_code);

                Log.Information($"Sending {request.Method} request to {url}");

                var response = client.Execute(request);

                if (response is null)
                {
                    Message.DisplaySTA("Error", "Failed to execute authenticate to epic games server.", discord: true, solutions: new[] { "Disable Windows Defender Firewall", "Disable any anti-virus softwares" });
                    return false;
                }

                if (response.StatusCode != System.Net.HttpStatusCode.OK || response.Content is null || !response.Content.ValidJson())
                {
                    Message.DisplaySTA("Error", "Authorization code is invalid. Please make sure you copied and\npasted the code correctly and that it has not expired!", discord: true);
                    return false;
                }

                var parse = JsonConvert.DeserializeObject<JObject>(response.Content);

                AccessToken = parse["access_token"].Value<string>();
                AccountID = parse["account_id"].Value<string>();

                Log.Information($"Finished {request.Method} request in {stopwatch.GetElaspedAndStop().ToString("mm':'ss")} received {response.Content.Length}");

                return true;
            }
        }

        public bool Download()
        {
            var stopwatch = new Stopwatch(); stopwatch.Start();
            string url = $"https://fngw-mcp-gc-livefn.ol.epicgames.com/fortnite/api/cloudstorage/user/{AccountID}/ClientSettings.Sav";

            using (var client = new RestClient())
            {
                var request = new RestRequest(new Uri(url), Method.Get);
                request.AddHeader("Authorization", $"Bearer {AccessToken}");

                Log.Information($"Sending {request.Method} request to {url}");

                var response = client.Execute(request);

                if (response is null)
                {
                    Message.DisplaySTA("Error", "Failed to download 'ClientSettings.Sav' file from epic games server!", discord: true, solutions: new[] { "Disable Windows Defender Firewall", "Disable any anti-virus softwares" });
                    return false;
                }

                if (response.StatusCode != System.Net.HttpStatusCode.OK || response.RawBytes is null)
                {
                    Message.DisplaySTA("Error", "Authorization code is expired. Please make sure you copied and\npasted the code correctly and that it has not expired!", discord: true);
                    return false;
                }

                Buffer = response.RawBytes;

                Log.Information($"Finished {request.Method} request in {stopwatch.GetElaspedAndStop().ToString("mm':'ss")} received {response.Content.Length}");

                return true;
            }
        }

        public bool Upload(byte[] clientSettingsBuffer)
        {
            var stopwatch = new Stopwatch(); stopwatch.Start();
            string url = $"https://fngw-mcp-gc-livefn.ol.epicgames.com/fortnite/api/cloudstorage/user/{AccountID}/ClientSettings.Sav";

            using (var client = new RestClient())
            {
                var request = new RestRequest(new Uri(url), Method.Put);
                request.AddHeader("Authorization", $"Bearer {AccessToken}");
                request.AddParameter("application/octet-stream", clientSettingsBuffer, ParameterType.RequestBody);

                Log.Information($"Sending {request.Method} request to {url}");

                var response = client.Execute(request);

                if (response is null)
                {
                    Message.DisplaySTA("Error", "Failed to upload 'ClientSettings.Sav' file to epic games server!", discord: true, solutions: new[] { "Disable Windows Defender Firewall", "Disable any anti-virus softwares" });
                    return false;
                }

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Message.DisplaySTA("Error", "Authorization code is expired. Please make sure you copied and\npasted the code correctly and that it has not expired!", discord: true);
                    return false;
                }

                Log.Information($"Finished {request.Method} request in {stopwatch.GetElaspedAndStop().ToString("mm':'ss")} received {response.Content.Length}");

                return true;
            }
        }

        #region Compression
        private static byte[] Compress(byte[] inputData)
        {
            using (var compressedStream = new MemoryStream())
            {
                using (var deflaterStream = new DeflaterOutputStream(compressedStream))
                {
                    deflaterStream.Write(inputData, 0, inputData.Length);
                }

                return compressedStream.ToArray();
            }
        }
        private static byte[] Decompress(byte[] compressedData, int decompressedSize)
        {
            using (var decompressedStream = new MemoryStream(decompressedSize))
            {
                using (var compressedStream = new MemoryStream(compressedData))
                {
                    using (var inflaterStream = new InflaterInputStream(compressedStream))
                    {
                        inflaterStream.CopyTo(decompressedStream);
                        decompressedStream.Seek(0, SeekOrigin.Begin);

                        return decompressedStream.ToArray();
                    }
                }
            }
        }
        #endregion
    }
}