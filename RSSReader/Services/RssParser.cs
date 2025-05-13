using System;
using System.Security.Policy;
using System.ServiceModel.Syndication;
using System.Xml;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;
using RSSReader.Data;
using RSSReader.Models;
using RSSReader.Services;

namespace RSSReader.Services
{
    /// <summary>
    /// Сервис преобразования RSS-фидов в доменные модели.
    /// Основное назначение: парсинг XML-фидов и нормализация данных.
    /// 
    /// Функции:
    /// - Чтение и валидация RSS
    /// - Преобразование элементов фида в единый формат
    /// - Извлечение изображений (если присутствуют)
    public class RssParser
    {
        private readonly RssDiscoverer _discoverer;
        public RssParser(RssDiscoverer discoverer)
        {
            _discoverer = discoverer;
        }

        public async Task<Feed> ParseFeedAsync(string feedUrl)
        {

            List<string> rssLinks = await _discoverer.FindRssFeedsAsync(feedUrl);
            string foundfeedUrl = rssLinks.Any() ? rssLinks.First() : feedUrl;
            using var reader = XmlReader.Create(foundfeedUrl);
            var syndicationFeed = SyndicationFeed.Load(reader);
            string imageUrl = syndicationFeed.ImageUrl?.ToString() ?? "";

            var feed = new Feed
            {
                Url = foundfeedUrl,
                Title = syndicationFeed.Title?.Text ?? foundfeedUrl,
                Description = syndicationFeed.Description?.Text ?? foundfeedUrl,
                ImageUrl = imageUrl,
                FeedItems = syndicationFeed.Items.Select(item => new FeedItem
                {
                    Title = item.Title.Text,
                    Description = item.Summary?.Text ?? "",
                    PublishDate = item.PublishDate.DateTime,
                    Link = item.Links.FirstOrDefault()?.Uri.ToString() ?? ""
                }).ToList()
            };
            
            return feed;
        }
    }
}

