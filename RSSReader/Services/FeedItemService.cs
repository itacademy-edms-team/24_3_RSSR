using Microsoft.EntityFrameworkCore;
using RSSReader.Data;
using RSSReader.Models;

namespace RSSReader.Services
{
    /// <summary>
    /// Сервис работы с новостными записями RSS-лент.
    /// Основное назначение: управление коллекцией новостей.
    /// 
    /// Функции:
    /// - Массовое обновление новостей из всех лент
    /// - Фильтрация и сортировка новостей
    /// - Поиск по содержимому
    public class FeedItemService
    {
        private readonly AppDbContext _context;
        private readonly RssParser _parser;

        public FeedItemService(AppDbContext context, RssParser parser)
        {
            _context = context;
            _parser = parser;
        }

        public async Task UpdateFeedsAsync()
        {
            var feeds = await _context.Feeds.ToListAsync();

            foreach (var feed in feeds)
            {
                try
                {
                    var newItems = await _parser.ParseFeedAsync(feed.Url);

                    foreach (var item in newItems.FeedItems)
                    {
                        if (!await _context.FeedItems.AnyAsync(i => i.Link == item.Link))
                        {
                            item.FeedId = feed.FeedId;
                            _context.FeedItems.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при обновлении ленты {feed.Url}: {ex.Message}");
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<FeedItem>> GetFeedItemsAsync(
            string searchQuery = null,
            bool hasImages = false,
            bool withoutImages = false,
            int? lastDays = null,
            string sortOrder = "date_desc")
        {
            var query = _context.FeedItems.Include(i => i.Feed).AsQueryable();

            ApplyFilters(ref query, searchQuery, hasImages, withoutImages, lastDays);
            ApplySorting(ref query, sortOrder);

            return await query.ToListAsync();
        }

        private void ApplyFilters(
            ref IQueryable<FeedItem> query,
            string searchQuery,
            bool hasImages,
            bool withoutImages,
            int? lastDays)
        {
            if (hasImages)
            {
                query = query.Where(i => i.Description != null &&
                   (i.Description.Contains("<img") ||
                    i.Description.Contains("<image") ||
                    i.Description.Contains("<picture")));
            }

            if (withoutImages)
            {
                query = query.Where(i => i.Description != null &&
                   !(i.Description.Contains("<img") ||
                    i.Description.Contains("<image") ||
                    i.Description.Contains("<picture")));
            }

            if (lastDays.HasValue)
                query = query.Where(i => i.PublishDate >= DateTime.Now.AddDays(-lastDays.Value));

            if (!string.IsNullOrEmpty(searchQuery))
                query = query.Where(i =>
                    EF.Functions.Like(i.Title, $"%{searchQuery}%") ||
                    EF.Functions.Like(i.Description, $"%{searchQuery}%"));
        }
        private void ApplySorting(ref IQueryable<FeedItem> query, string sortOrder)
        {
            switch (sortOrder)
            {
                case "date_desc":
                    query = query.OrderByDescending(i => i.PublishDate);
                    break;
                case "date_asc":
                    query = query.OrderBy(i => i.PublishDate);
                    break;
                case "title":
                    query = query.OrderBy(i => i.Title);
                    break;
                case "title_desc":
                    query = query.OrderByDescending(i => i.Title);
                    break;
                default:
                    query = query.OrderByDescending(i => i.PublishDate);
                    break;
            }
        }

        public List<string> GetActiveFilters(
            string searchQuery,
            bool hasImages,
            bool withoutImages,
            int? lastDays)
        {
            var activeFilters = new List<string>();

            if (hasImages)
                activeFilters.Add("С изображениями");

            if (withoutImages)
                activeFilters.Add("Без изображений");

            if (lastDays.HasValue)
                activeFilters.Add($"Последние {lastDays} дней");

            if (!string.IsNullOrEmpty(searchQuery))
                activeFilters.Add($"Поиск: '{searchQuery}'");

            return activeFilters;
        }
    }

}
