using System.Reflection;

namespace WLEDAnimated;

public class Version
{
    private readonly string _version;

    public Version()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var informationalVersionAttribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        _version = informationalVersionAttribute != null
            ? informationalVersionAttribute.InformationalVersion
            : "1.0.DEADBEEF";

        var versionParts = _version.Split('.');

        FullVersion = _version;

        if (versionParts.Length >= 3)
        {
            Major = versionParts[0];
            Minor = versionParts[1];
            Hash = versionParts[2];
        }
        else
        {
            // Handle unexpected format
            Major = "1";
            Minor = "0";
            Hash = "DEADBEEF";
        }
    }

    public string Major { get; }
    public string Minor { get; }
    public string Hash { get; }
    public string FullVersion { get; }
}