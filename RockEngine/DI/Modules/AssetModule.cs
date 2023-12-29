using Ninject.Modules;

using OpenTK.Mathematics;

using RockEngine.ECS;
using RockEngine.ECS.Assets;
using RockEngine.ECS.Assets.Converters;
using RockEngine.Rendering.OpenGL.Settings;

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

           
        }
    }
}
