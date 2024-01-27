using System.Reflection;
using System.Runtime.Loader;

namespace WLEDAnimated.API;

public class AssemblyTypeProcessor
{
    public List<Type> ProcessTypesImplementingInterface(string directoryPath, Type interfaceType)
    {
        List<Type> typesImplementingInterface = new List<Type>();

        // Load each assembly in the specified directory
        foreach (var filePath in Directory.GetFiles(directoryPath, "*.dll"))
        {
            try
            {
                Assembly assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(filePath);
                var types = assembly.GetTypes()
                    .Where(t => interfaceType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                typesImplementingInterface.AddRange(types);
            }
            catch
            {
                // safe to skip since we only care about loadable plugins.
            }
        }

        return typesImplementingInterface;
    }
}