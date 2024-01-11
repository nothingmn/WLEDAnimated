namespace WLEDAnimated.API;

public enum AssetTypes
{
    Test,
    Weather
}

public class AssetManager
{
    public System.IO.DirectoryInfo AssetsDirectory { get; set; }

    public AssetManager()
    {
        var asmLocation = new FileInfo(this.GetType().Assembly.Location);
        AssetsDirectory = new System.IO.DirectoryInfo(System.IO.Path.Combine(asmLocation.Directory.FullName, "Assets"));
        if (!AssetsDirectory.Exists) AssetsDirectory.Create();

        EnsureFolderExists(System.Enum.GetName(AssetTypes.Test).ToLowerInvariant(), AssetsDirectory);
        EnsureFolderExists(System.Enum.GetName(AssetTypes.Test).ToLowerInvariant(), AssetsDirectory);
    }

    private DirectoryInfo EnsureFolderExists(string name, DirectoryInfo AssetsDirectory)
    {
        var folder = new System.IO.DirectoryInfo(System.IO.Path.Combine(AssetsDirectory.FullName, name));
        if (!folder.Exists) folder.Create();
        return folder;
    }

    public List<DirectoryInfo> GetResolutionsByAssetType(AssetTypes type)
    {
        var assetDirectory = new DirectoryInfo(System.IO.Path.Combine(AssetsDirectory.FullName, System.Enum.GetName(type).ToLowerInvariant()));
        return assetDirectory.GetDirectories().ToList();
    }

    public List<FileInfo> GetFilesByAssetTypeAndResolution(AssetTypes type, int width, int height)
    {
        var resolutions = GetResolutionsByAssetType(type);
        var files = new List<FileInfo>();
        foreach (var resolution in resolutions)
        {
            var file = resolution.GetFiles();
            if (file != null)
            {
                files.AddRange(file);
            }
        }
        return files;
    }

    public FileInfo GetFileByAssetTypeAndResolutionAndName(AssetTypes type, int width, int height, string name)
    {
        var files = GetFilesByAssetTypeAndResolution(type, height, width);
        return files.FirstOrDefault(x => x.Name.ToLowerInvariant().StartsWith(name?.ToLowerInvariant()));
    }
}