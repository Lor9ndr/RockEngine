using System.Reflection;

namespace RockEngine.DI
{
    public class RegistratorLoader
    {
        public static void LoadRegistrators(Assembly assembly)
        {
            var registratorTypes = assembly.GetTypes()
                .Where(t => typeof(IRegistrator).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach(var registratorType in registratorTypes)
            {
                var registrator = (IRegistrator)Activator.CreateInstance(registratorType);
                registrator.Register();
            }
        }
    }
}
