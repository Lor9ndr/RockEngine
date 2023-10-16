using Ninject.Modules;

using OpenTK.Mathematics;

using RockEngine.Assets;
using RockEngine.Assets.AssetCreators;
using RockEngine.Assets.Converters;
using RockEngine.Engine;
using RockEngine.Engine.ECS;
using RockEngine.Engine.ECS.GameObjects;
using RockEngine.OpenGL.Settings;
using RockEngine.OpenGL.Textures;

namespace RockEngine.DI.Modules
{
    internal sealed class AssetModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IConverter<BaseAsset>>().To<BaseAssetConverter>();
            Kernel.Bind<IConverter<TextureSettings>>().To<TextureSettingsConverter>();
            Kernel.Bind<IConverter<GameObject>>().To<GameObjectConverter>();
            Kernel.Bind<IConverter<Vector3>>().To<Vector3Converter>();

            Kernel.Bind<IAssetCreator<BaseAsset>>().To<BaseAssetCreator>();
            Kernel.Bind<IAssetCreator<Project>>().To<ProjectAssetCreator>();
            Kernel.Bind<IAssetCreator<Scene>>().To<SceneAssetCreator>();
            Kernel.Bind<IAssetCreator<Texture>>().To<TextureAssetCreator>();
            Kernel.Bind<IAssetCreator<Texture2D>>().To<Texture2DAssetCreator>();
            Kernel.Bind<IAssetCreator<MaterialComponent>>().To<MaterialAssetCreator>();
        }
    }
}
