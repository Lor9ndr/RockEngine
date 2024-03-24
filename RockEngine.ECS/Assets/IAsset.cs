using Newtonsoft.Json;

namespace RockEngine.ECS.Assets
{
    public interface IAsset
    {
        /// <summary>
        /// Path to Asset file
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// Name of asset object
        /// </summary>
        string Name { get; set; }

        [JsonRequired]
        AssetType Type { get; }

        /// <summary>
        /// ID of asset object
        /// </summary>
        Guid ID { get; set; }

        /// <summary>
        /// Means that is overriden after save
        /// </summary>
        bool IsDirty { get; set; }

        /// <summary>
        /// Call when ready to use in OpenGL thread
        /// </summary>
        public void Loaded();
    }
}
