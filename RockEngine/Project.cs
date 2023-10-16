using RockEngine.Assets;
using RockEngine.Engine;

using System.Text.Json.Serialization;

namespace RockEngine
{
    internal sealed class Project : BaseAsset, IDisposable
    {
        public static Project? CurrentProject { get; set; }

        public static Scene CurrentScene => Scene.CurrentScene;

        [JsonIgnore]
        public IEnumerable<Scene> Scenes => AssetManager.Assets.OfType<Scene>();

        public Project(string name, string path, Guid id)
            : base(path, name, id, AssetType.Project)
        {
            CurrentProject = this;
            Scene scene = new Scene("Default scene", PathInfo.PROJECT_ASSETS_PATH, Guid.NewGuid());
            Scene.ChangeScene(scene);
        }

        public Project(BaseAsset baseAsset)
            : base(baseAsset.Path, baseAsset.Name, baseAsset.ID, AssetType.Project)
        {
            CurrentProject = this;
            Scene scene = new Scene("Default scene", PathInfo.PROJECT_ASSETS_PATH, Guid.NewGuid());
            Scene.ChangeScene(scene);
        }

        public Project()
        {

        }

        public void Dispose()
        {
            foreach (var item in AssetManager.Assets)
            {
                if (item is IDisposable disp)
                {
                    disp.Dispose();
                }
            }
            Scene.ChangeScene(null);
        }
    }
}
