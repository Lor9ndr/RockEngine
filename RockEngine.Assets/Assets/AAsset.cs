
using Newtonsoft.Json;

namespace RockEngine.Assets.Assets
{
    public abstract class AAsset : IAsset
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public Guid ID { get; set; }

        public abstract AssetType Type { get; internal set; }

        [JsonIgnore]
        public bool IsDirty { get; set; }

        protected AAsset(string path, string name, Guid id)
        {
            Path = path;
            Name = name;
            ID = id;
        }
        protected AAsset(string path, string name)
        {
            Path = path;
            Name = name;
            ID = Guid.NewGuid();

        }
        protected AAsset()
        {
            Path = string.Empty;
            Name = "Asset";
            ID = Guid.NewGuid();
        }
    }
}
