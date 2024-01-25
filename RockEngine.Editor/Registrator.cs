using Ninject;

using RockEngine.DI;
using RockEngine.Editor.Physics;
using RockEngine.Physics;

namespace RockEngine.Editor
{
    public sealed class Registrator : ARegistrator
    {
        public Registrator(StandardKernel kernel)
            : base(kernel)
        {
        }

        public override void Register()
        {
            _kernel.Bind<IWorldRenderer>().To<DebugPhysicsRenderer>().InSingletonScope();
        }
    }
}
