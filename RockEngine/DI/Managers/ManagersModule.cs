using Ninject.Modules;

using RockEngine.Physics;

namespace RockEngine.DI.Managers
{
    internal sealed class ManagersModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<PhysicsManager>().ToSelf().InSingletonScope();
        }
    }
}
