using System.ServiceModel.Syndication;
using System.Xml;
using RSSReader.Models;

namespace RSSReader.Services
{
    public class RssParser
    {
        public List<FeedItem> ParseFeed(string url)
        {
            try
            {
                using (var reader = XmlReader.Create(url))
                {
                    var feed = SyndicationFeed.Load(reader);
                    return feed.Items.Select(item => new FeedItem
                    {
                        Title = item.Title?.Text ?? "Без заголовка",
                        Description = item.Summary?.Text ?? "Описание отсутствует",
                        PublishDate = item.PublishDate.DateTime,
                        Link = item.Links?.FirstOrDefault()?.Uri?.ToString() ?? "#"
                    }).ToList();
                }
            }
            catch
            {
                return new List<FeedItem>();
            }
        }
    }
}