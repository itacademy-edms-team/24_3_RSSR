namespace RSSReader.Models
{
    public class Feed
    {
        public int FeedId { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; } = "";

        public ICollection<FeedItem> FeedItems { get; set; }
        public ICollection <Folder> Folders { get; set; }
    }
}
