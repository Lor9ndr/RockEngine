using Ninject;

using System.Reflection;

namespace RockEngine.DI
{
    public class RegistratorLoader
    {
        public static void LoadRegistrators(Assembly assembly, StandardKernel kernel)
        {
            var registratorTypes = assembly.GetTypes()
                .Where(t => typeof(ARegistrator).IsAssignableFrom(t)  && !t.IsAbstract);

            foreach(var registratorType in registratorTypes)
            {
                var registrator = (ARegistrator)Activator.CreateInstance(registratorType, kernel);
                registrator.Register();
            }
        }
    }
}
