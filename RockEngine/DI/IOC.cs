using Ninject;
using Ninject.Modules;
using Ninject.Parameters;

using RockEngine.DI.Managers;
using RockEngine.DI.Modules;

using System.Reflection;

namespace RockEngine.DI
{
    public static class IoC
    {
        private static StandardKernel? _kernel;

        public static void Setup()
        {
            _kernel = new StandardKernel();
            _kernel.Load(new AssetModule(), new ManagersModule());

            RegistratorLoader.LoadRegistrators(Assembly.GetEntryAssembly(), _kernel);
        }

        public static void Load(NinjectModule module)
        {
            _kernel.Load(module);
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

        public static T Get<T>(ConstructorArgument argument)
        {
            return _kernel.Get<T>(argument);
        }
    }
}
