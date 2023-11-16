namespace EmeraldSDK.Models;

public class Game
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public Uri? IconUri { get; set; }
    public Uri? ArtworkUri { get; set; }
    public Uri? CoverUri { get; set; }
}