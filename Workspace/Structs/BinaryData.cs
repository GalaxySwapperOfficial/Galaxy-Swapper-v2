namespace Galaxy_Swapper_v2.Workspace.Structs
{
    public class BinaryData
    {
        public string Hash { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public BinaryData(string hash, string name, string path)
        {
            Hash = hash;
            Name = name;
            Path = path;
        }
    }
}