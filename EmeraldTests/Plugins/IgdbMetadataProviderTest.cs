using EmeraldSDK.Models;
using EmeraldSDK.Plugins.DefaultPlugins;

namespace EmeraldTests.Plugins;

public class IgdbMetadataProviderTest
{
    [Fact]
    public async Task InitializeAsync_Success()
    {
        var igdbMetadataProvider = new IgdbMetadataProvider();

        try
        {
            await igdbMetadataProvider.InitializeAsync();
            await Task.Delay(1000);
        }
        catch (Exception e)
        {
            Assert.True(false, $"No exception should be thrown, but {e.GetType()} was thrown.");
        }
        
        Assert.True(true);
    }
    
    [Fact]
    public async Task SearchGameAsync_Success()
    {
        var igdbMetadataProvider = new IgdbMetadataProvider();
        IEnumerable<Game> games = new List<Game>();
        
        try
        {
            await igdbMetadataProvider.InitializeAsync();
            await Task.Delay(1000);
            
            games = await igdbMetadataProvider.SearchGameAsync("Halo");
        }
        catch (Exception e)
        {
            Assert.True(false, $"No exception should be thrown, but {e.GetType()} was thrown.");
        }
        Assert.True(games.Any());
    }
}