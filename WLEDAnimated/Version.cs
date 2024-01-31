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

        if (_version.Contains("+")) _version = _version.Replace("+", "-");
        var _hash = _version.Split('-');
        if (_hash[1].Length > 7)
        {
            _version = $"{_hash[0]}-{_hash[1].Substring(0, 7)}";
        }

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
            Hash = "DEADBEEFFF";
        }
    }

    public string Major { get; }
    public string Minor { get; }
    public string Hash { get; }
    public string FullVersion { get; }
}