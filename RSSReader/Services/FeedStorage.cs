using System.Text.Json;

namespace RSSReader.Services
{
    public class FeedStorage
    {
        private const string FeedsFile = "feeds.json";
        public List<string> LoadFeeds() =>
            File.Exists(FeedsFile) ? JsonSerializer.Deserialize<List<string>>(File.ReadAllText(FeedsFile)) : new();

        public void SaveFeeds(List<string> feeds)
        {
            var uniqueFeeds = feeds.Distinct().ToList();
            File.WriteAllText(FeedsFile, JsonSerializer.Serialize(uniqueFeeds));
        }
    }
}
