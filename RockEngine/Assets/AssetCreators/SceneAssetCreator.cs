using RockEngine.Assets.Converters;
using RockEngine.Engine;

namespace RockEngine.Assets.AssetCreators
{
    internal sealed class SceneAssetCreator : AAssetCreator<Scene>
    {
        private readonly GameObjectConverter _gameobjectConverter;
        private readonly BaseAssetConverter _baseConverter;

        public SceneAssetCreator(GameObjectConverter converter,
            BaseAssetConverter baseConverter)
        {
            _gameobjectConverter = converter;
            _baseConverter = baseConverter;
        }
        public override Scene Load(string path)
        {
            using var reader = new BinaryReader(File.Open(path, FileMode.OpenOrCreate));
            var baseScene = _baseConverter.Read(reader);
            Scene scene = new Scene(baseScene.Name, baseScene.Path, baseScene.ID);
            var countGameObjects = reader.ReadInt32();
            for (int i = 0; i < countGameObjects; i++)
            {
                var go = _gameobjectConverter.Read(reader);
                scene.AddGameObject(go);
            }
            return scene;
        }

        public override void Save<TAsset>(TAsset asset)
        {
            var scene = asset as Scene;
            using var writer = new BinaryWriter(File.Open(GetFullPath(scene), FileMode.OpenOrCreate));
            _baseConverter.Write(scene, writer);
            var gameobjects = scene.GetGameObjects();
            writer.Write(gameobjects.Count);
            foreach (var item in gameobjects)
            {
                _gameobjectConverter.Write(item, writer);
            }
        }
    }
}
