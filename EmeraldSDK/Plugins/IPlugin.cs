namespace EmeraldSDK.Plugins;

public interface IPlugin
{
    string Name { get; }
    string Description { get; }
    Uri? IconUri { get; }
    string? Version { get; }
    
    Task InitializeAsync();
}