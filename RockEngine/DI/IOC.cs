using Ninject;

using RockEngine.DI.Managers;
using RockEngine.DI.Modules;

namespace RockEngine.DI
{
    public static class IoC
    {
        private static StandardKernel? _kernel;

        public static void Setup()
        {
            _kernel = new StandardKernel();
            _kernel.Load(new AssetModule(), new ManagersModule());
        }

        public static T Get<T>()
        {
            return _kernel.Get<T>();
        }

        public static IEnumerable<T> GetAll<T>()
        {
            return _kernel.GetAll<T>();
        }

        public static object Get(Type t)
        {
            return _kernel.Get(t);
        }
    }
}
