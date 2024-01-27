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
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading assembly '{filePath}': {ex.Message}");
                // Optionally handle or log exceptions
            }
        }

        return typesImplementingInterface;
    }
}