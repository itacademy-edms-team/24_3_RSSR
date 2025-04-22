namespace RSSReader.Models
{
    public class Feed
    {
        public int FeedId { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }

        public FeedItem? FeedItem { get; set; }
        public ICollection <Folder> Folder { get; set; }
    }
}
