using Galaxy_Swapper_v2.Workspace.Compression;
using Galaxy_Swapper_v2.Workspace.Hashes;
using Galaxy_Swapper_v2.Workspace.Swapping.Compression.Types;
using Galaxy_Swapper_v2.Workspace.Swapping.Other;
using Galaxy_Swapper_v2.Workspace.Utilities;
using RestSharp;
using Serilog;
using System;
using System.Diagnostics;
using System.Net;
using static Galaxy_Swapper_v2.Workspace.Global;

namespace Galaxy_Swapper_v2.Workspace.Swapping.Providers
{
    public static class StreamDataProvider
    {
        private const string Domain = "https://galaxyswapperv2.com/API/StreamData/{0}.chunk";
        public enum CompressionType
        {
            None = 0,
            Aes = 1,
            Zlib = 2,
            Oodle = 3,
            GZip = 4
        }

        public static byte[] Download(string path)
        {
            var stopwatch = new Stopwatch(); stopwatch.Start();
            string chunkName = Convert.ToHexString(MD5.Hash(path));
            string url = string.Format(Domain, chunkName);

            using (var client = new RestClient())
            {
                var request = new RestRequest(new Uri(url), Method.Get);

                Log.Information($"Sending {request.Method} request to {url}");

                var response = client.Execute(request);

                if (response.StatusCode != HttpStatusCode.OK || response.RawBytes is null || response.RawBytes.Length == 0)
                {
                    Log.Fatal($"Failed to download response from StreamData! Expected: {HttpStatusCode.OK} Received: {response.StatusCode}");
                    Message.DisplaySTA("Error", "Webclient caught a exception while downloading StreamData.", discord: true, solutions: new[] { "Disable Windows Defender Firewall", "Disable any anti-virus softwares", "Turn on a VPN" });
                    return null!;
                }

                Log.Information($"Finished {request.Method} request in {stopwatch.GetElaspedAndStop().ToString("mm':'ss")} received {response.Content.Length}");

                var reader = new Reader(response.RawBytes); reader.BaseStream.Position += 4;
                var compressionType = (CompressionType)reader.Read<int>();

                switch (compressionType)
                {
                    case CompressionType.None:
                        return response.RawBytes;
                    case CompressionType.Aes:
                        {
                            ulong keyHash = reader.Read<ulong>();
                            int keyLength = reader.Read<int>();
                            byte[] key = reader.ReadBytes(keyLength);

                            if (CityHash.Hash(key) != keyHash)
                            {
                                Log.Error($"StreamData encryption key hash miss match");
                                throw new CustomException($"StreamData encryption key hash miss match");
                            }

                            ulong encryptedHash = reader.Read<ulong>();
                            ulong unencryptedHash = reader.Read<ulong>();
                            int encryptedLength = reader.Read<int>();
                            byte[] encryptedBuffer = reader.ReadBytes(encryptedLength);

                            if (CityHash.Hash(encryptedBuffer) != encryptedHash)
                            {
                                Log.Error($"StreamData encryptedBuffer hash miss match");
                                throw new CustomException($"StreamData encryptedBuffer hash miss match");
                            }

                            byte[] unencryptedBuffer = aes.Decrypt(encryptedBuffer, key);

                            if (CityHash.Hash(unencryptedBuffer) != unencryptedHash)
                            {
                                Log.Error($"StreamData unencryptedBuffer hash miss match");
                                throw new CustomException($"StreamData unencryptedBuffer hash miss match");
                            }

                            return unencryptedBuffer;
                        }
                    case CompressionType.Zlib:
                        {
                            ulong compressedHash = reader.Read<ulong>();
                            ulong uncompressedHash = reader.Read<ulong>();
                            int compressedSize = reader.Read<int>();
                            int uncompressedSize = reader.Read<int>();

                            byte[] compressedBuffer = reader.ReadBytes(compressedSize);

                            if (compressedHash != CityHash.Hash(compressedBuffer))
                            {
                                Log.Error($"StreamData compressedHash miss match");
                                throw new CustomException($"StreamData compressedHash miss match!");
                            }

                            byte[] uncompressedBuffer = zlib.Decompress(compressedBuffer, uncompressedSize);

                            if (uncompressedHash != CityHash.Hash(uncompressedBuffer))
                            {
                                Log.Error($"StreamData uncompressedHash miss match");
                                throw new CustomException($"StreamData uncompressedHash miss match!");
                            }
                            return uncompressedBuffer;
                        }
                    case CompressionType.Oodle:
                        {
                            ulong compressedHash = reader.Read<ulong>();
                            ulong uncompressedHash = reader.Read<ulong>();
                            int compressedSize = reader.Read<int>();
                            int uncompressedSize = reader.Read<int>();

                            byte[] compressedBuffer = reader.ReadBytes(compressedSize);

                            if (compressedHash != CityHash.Hash(compressedBuffer))
                            {
                                Log.Error($"StreamData compressedHash miss match");
                                throw new CustomException($"StreamData compressedHash miss match!");
                            }

                            byte[] uncompressedBuffer = Oodle.Decompress(compressedBuffer, uncompressedSize);

                            if (uncompressedHash != CityHash.Hash(uncompressedBuffer))
                            {
                                Log.Error($"StreamData uncompressedHash miss match");
                                throw new CustomException($"StreamData uncompressedHash miss match!");
                            }
                            return uncompressedBuffer;
                        }
                    case CompressionType.GZip:
                        {
                            ulong compressedHash = reader.Read<ulong>();
                            ulong uncompressedHash = reader.Read<ulong>();
                            int compressedSize = reader.Read<int>();
 

                            byte[] compressedBuffer = reader.ReadBytes(compressedSize);

                            if (compressedHash != CityHash.Hash(compressedBuffer))
                            {
                                Log.Error($"StreamData compressedHash miss match");
                                throw new CustomException($"StreamData compressedHash miss match!");
                            }

                            byte[] uncompressedBuffer = gzip.Decompress(compressedBuffer);

                            if (uncompressedHash != CityHash.Hash(uncompressedBuffer))
                            {
                                Log.Error($"StreamData uncompressedHash miss match");
                                throw new CustomException($"StreamData uncompressedHash miss match!");
                            }
                            return uncompressedBuffer;
                        }
                    default:
                        Log.Error($"StreamData unknown compression type");
                        throw new CustomException($"StreamData unknown compression type");
                }
            }
        }
    }
}