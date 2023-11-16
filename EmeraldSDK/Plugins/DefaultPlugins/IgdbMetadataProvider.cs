using System.Text;
using EmeraldSDK.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EmeraldSDK.Plugins.DefaultPlugins;

public class IgdbMetadataProvider : IMetadataProviderPlugin
{
    public string Name { get; } = "IGDB";
    public string Description { get; } = "Provide metadata from IGDB.";
    public Uri? IconUri { get; } = null;
    public string? Version { get; } = null;

    private const string ClientId = "khjys068j3d1a8e2f2cj9cinca3lzb";
    private const string ClientSecret = "yc827jqapk3qn7134j93h4tb8a2pj6";

    private string? _twitchAccessToken;

    private async Task UpdateTwitchAccessTokenAsync()
    {
        while (true)
        {
            var client = new HttpClient();
            var query = new Dictionary<string, string>
            {
                { "client_id", ClientId }, { "client_secret", ClientSecret }, { "grant_type", "client_credentials" }
            };

            var content = new FormUrlEncodedContent(query);
            var response = await client.PostAsync("https://id.twitch.tv/oauth2/token", content);
            if (!response.IsSuccessStatusCode)
            {
                // TODO
                // Write in log
                throw new HttpRequestException("Failed to get Twitch access token.");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseJson = JObject.Parse(responseContent);
            if (responseJson == null) throw new JsonException("Failed to deserialize Twitch access token.");

            if (responseJson.TryGetValue("access_token", out var accessToken))
            {
                if (accessToken.Type != JTokenType.String || accessToken.Value<string>() == null)
                    throw new JsonException("Failed to deserialize Twitch access token.");
                _twitchAccessToken = accessToken.Value<string>();
            }

            if (responseJson.TryGetValue("expires_in", out var lifeTime)) // Handle token expiration
            {
                if(lifeTime.Type != JTokenType.Integer || lifeTime == null)
                    throw new JsonException("Failed to deserialize Twitch access token.");
                await Task.Delay(lifeTime.Value<int>() * 1000);
                continue;
            }

            break;
        }
    }

    public Task InitializeAsync()
    {
        _ = UpdateTwitchAccessTokenAsync();
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<Game>> SearchGameAsync(string query)
    {
        if (_twitchAccessToken == null) throw new NullReferenceException("Twitch access token is null.");
        
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Client-ID", ClientId);
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_twitchAccessToken}");
        
        var requestBody = $"fields name, summary, cover.url, artworks.url; search \"{query}\"; where parent_game = null & version_parent = null & first_release_date != null;";
        var content = new StringContent(requestBody, Encoding.UTF8, "text/plain");
        var response = await client.PostAsync("https://api.igdb.com/v4/games/", content);
        
        if (!response.IsSuccessStatusCode) throw new HttpRequestException("Failed to get game from IGDB.");
        
        var stringData = await response.Content.ReadAsStringAsync();
        var jsonResults = JArray.Parse(stringData);
        
        var resultGames = new List<Game>();
        foreach (var result in jsonResults)
        {
            var title = result["name"]?.Value<string>();
            var description = result["summary"]?.Value<string>();
            
            var coverUriString = result["cover"]?["url"]?.Value<string>()?.Replace("t_thumb", "t_cover_big");
            var coverUri = coverUriString != null ? new Uri(coverUriString) : null;
            
            var artwork = result["artworks"]?.FirstOrDefault()?["url"]?.Value<string>()?.Replace("t_thumb", "t_1080p");
            var artworkUri = artwork != null ? new Uri(artwork) : null;
            
            Game game = new()
            {
                Title = title,
                Description = description,
                CoverUri = coverUri,
                ArtworkUri = artworkUri
            };
            
            resultGames.Add(game);
        }

        return resultGames;
    }
}