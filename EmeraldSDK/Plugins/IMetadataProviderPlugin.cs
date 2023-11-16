using EmeraldSDK.Models;

namespace EmeraldSDK.Plugins;

public interface IMetadataProviderPlugin : IPlugin
{
    Task<IEnumerable<Game>> SearchGameAsync(string query);
}