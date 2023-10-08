using CUE4Parse.FileProvider;
using Galaxy_Swapper_v2.Workspace.Generation.Formats;
using Serilog;
using System.Collections.Generic;

namespace Galaxy_Swapper_v2.Workspace.Structs
{
    public class ProviderData
    {
        public DefaultFileProvider FileProvider = null!;
        public List<StreamData> OpenedStreamers = new();
        public bool Save = false;
        public bool SaveStreams = false;

        public void Dispose()
        {
            if (FileProvider is null)
                return;

            Log.Information($"Disposing {OpenedStreamers.Count} opened streams");
            if (OpenedStreamers is not null && OpenedStreamers.Count > 0)
            {
                foreach (var stream in OpenedStreamers)
                {
                    stream.Stream.Close();
                }
            }

            Log.Information("Disposing Providers");
            FileProvider.Dispose();

            Log.Information($"Setting as {null}");
            FileProvider = null!;
            OpenedStreamers = new();
            Save = false;
            SaveStreams = false;
        }
    }
}
