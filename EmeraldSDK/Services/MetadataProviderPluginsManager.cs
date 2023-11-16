using EmeraldSDK.Models;
using EmeraldSDK.Plugins;

namespace EmeraldSDK.Services;

public class MetadataProviderPluginsManager
{
    /// <summary>
    /// The list of all loaded metadata providers.
    /// </summary>
    private List<IMetadataProviderPlugin> _metadataProviders = new();
    
    /// <summary>
    /// The currently selected metadata provider.
    /// </summary>
    private IMetadataProviderPlugin? _selectedMetadataProvider;
    
    /// <summary>
    /// The names of all loaded metadata providers.
    /// </summary>
    public List<string> MetadataProviderNames => _metadataProviders.Select(x => x.Name).ToList();
    
    /// <summary>
    /// Change the selected metadata provider.
    /// </summary>
    /// <param name="metadataProviderName">The name of the metadata provider to select.</param>
    public void SelectMetadataProvider(string metadataProviderName)
    {
        _selectedMetadataProvider = _metadataProviders.FirstOrDefault(x => x.Name == metadataProviderName);
    }

    public void LoadPlugins()
    {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Search for a game using the selected metadata provider.
    /// </summary>
    /// <param name="query">Name of the game to search for.</param>
    /// <returns>A list of games matching the query.</returns>
    /// <exception cref="NullReferenceException">Thrown when no metadata provider is selected.</exception>
    public async Task<IEnumerable<Game>> SearchGameAsync(string query)
    {
        if (_selectedMetadataProvider == null)
        {
            throw new NullReferenceException("No metadata provider selected.");
        }
        
        return await _selectedMetadataProvider.SearchGameAsync(query);
    }
}