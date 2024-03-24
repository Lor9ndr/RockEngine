using Ninject;

using RockEngine.DI;
using RockEngine.Editor.Physics;
using RockEngine.Physics;
using RockEngine.Physics.Drawing;

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
            _kernel.Bind<ColliderRenderer>().To<DebugPhysicsRenderer>().InSingletonScope();
        }
    }
}
