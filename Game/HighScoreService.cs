using System.Text.Json;

namespace TheAdventure.Game;

// AI-generated
public class HighScoreService : IDisposable
{
    private static readonly string SavePath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                     "TheAdventureBlackjack", "highscore.json");

    private record SaveData(int HighScore, DateTime SavedAt);

    public int HighScore { get; private set; } = 0;

    private bool _disposed;

    // Load on construction — called once at startup
    public static async Task<HighScoreService> LoadAsync()
    {
        var svc = new HighScoreService();
        await svc.LoadInternalAsync();
        return svc;
    }

    private async Task LoadInternalAsync()
    {
        try
        {
            if (!File.Exists(SavePath)) return;

            await using var stream = File.OpenRead(SavePath);
            var data = await JsonSerializer.DeserializeAsync<SaveData>(stream);
            if (data is not null)
                HighScore = data.HighScore;
        }
        catch
        {
            // Corrupt or missing — start fresh
            HighScore = 0;
        }
    }

    // Save high score if current balance beats it
    public async Task TrySaveAsync(int balance)
    {
        if (balance <= HighScore) return;
        HighScore = balance;

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SavePath)!);
            var data = new SaveData(HighScore, DateTime.UtcNow);

            await using var stream = File.Create(SavePath);
            await JsonSerializer.SerializeAsync(stream, data,
                new JsonSerializerOptions { WriteIndented = true });
        }
        catch
        {
            // Ignore I/O errors — don't crash the game
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
// end AI-generated