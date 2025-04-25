namespace RSSReader.Models
{
    public class Feed
    {
        public int FeedId { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }

        public ICollection<FeedItem> FeedItems { get; set; }
        public ICollection <Folder> Folders { get; set; }
    }
}
