namespace RSSReader.Models
{
    public class FeedItem
    {
        public int FeedItemId { get; set; }
        public int FeedId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublishDate { get; set; }
        public bool IsRead { get; set; }
        public Feed Feed { get; set; }

    }
}
