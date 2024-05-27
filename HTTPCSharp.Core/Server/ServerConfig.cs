namespace HTTPCSharp.Core.Server;

public class ServerConfig
{
	public readonly string RootDirectory;
	public readonly string ResourcesPath;
	public readonly List<InternalResource> InternalResources;

	public ServerConfig(string rootDirectory, string resourcesPath, List<InternalResource> resources)
	{
		RootDirectory = rootDirectory;
		ResourcesPath = resourcesPath;
		InternalResources = resources;
	}
}

public class ConfigurationManager
{
	private static readonly Lazy<ConfigurationManager> LazyInstance = new(() => new ConfigurationManager());
	public ServerConfig ServerConfig { get; private set; }
	
	private ConfigurationManager()
	{
		string rootDirectory = Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.Parent!.FullName;
		string resourcesPath = Path.Combine(rootDirectory, "Resources", "site");
		
		ServerConfig = new ServerConfig(
			rootDirectory,
			resourcesPath,
			ResourceLoader.Load(resourcesPath)
		); 
	}

	public static ConfigurationManager Instance = LazyInstance.Value;

}