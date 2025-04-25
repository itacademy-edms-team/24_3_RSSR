using System;
using System.ServiceModel.Syndication;
using System.Xml;
using Microsoft.VisualBasic.FileIO;
using RSSReader.Models;
using RSSReader.Services;

namespace RSSReader.Services
{
    public class RssParser
    {
        public async Task<List<FeedItem>> ParseFeedAsync(string url)
        {
            try
            {
                List<string> rssLinks = await RssDiscoverer.FindRssFeedsAsync(url);
                string feedUrl = rssLinks.Any() ? rssLinks.First() : url;
                using (var reader = XmlReader.Create(feedUrl))
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

