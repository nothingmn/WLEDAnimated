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
            : "1.0.0-DEADBEEFFF";

        //_version = "1.0.0-a0bc1df";
        //_version = "1.0.0-a0bc1dfdeadbeef22233423532";
        //_version = "1.0.0";
        //_version = "1.0.0+55581a6522fb8b132c0ce8a3f597b6a1cabd2bc4";
        _version = _version.Replace("+", "-");

        FullVersion = _version;

        if (_version.Contains("-"))
        {
            var parts = _version.Split('-');
            Hash = parts[1];
            if (Hash.Length > 7) Hash = Hash.Substring(0, 7);
            var versionParts = parts[0].Split('.');
            Major = versionParts[0];
            Minor = versionParts[1];
            Revision = versionParts[2];
            FullVersion = $"{Major}.{Minor}.{Revision}-{Hash}";
        }
    }

    public string Major { get; }
    public string Minor { get; }
    public string Revision { get; }
    public string Hash { get; }
    public string FullVersion { get; }
}