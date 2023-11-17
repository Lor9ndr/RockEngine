using RockEngine.Assets;
using RockEngine.Engine;

namespace RockEngine
{
    public sealed class Project : BaseAsset, IDisposable
    {
        public static Project? CurrentProject { get; set; }

        public static Scene? CurrentScene => Scene.CurrentScene;

        public Scene FirstScene { get; set; }

        public Project(string name, string path, Guid id, Scene firstScene)
            : base(path, name, id, AssetType.Project)
        {
            CurrentProject = this;
            Scene.ChangeScene(firstScene);
            FirstScene = firstScene;
        }

        public Project(BaseAsset baseAsset)
            : base(baseAsset.Path, baseAsset.Name, baseAsset.ID, AssetType.Project)
        {
            CurrentProject = this;
        }

        public Project()
        {
            CurrentProject = this;
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
