using Ninject;

namespace RockEngine.DI
{
    public abstract class ARegistrator
    {
        protected StandardKernel _kernel;
        protected ARegistrator(StandardKernel kernel)
        {
            _kernel = kernel;
        }

        public abstract void Register();
    }
}
