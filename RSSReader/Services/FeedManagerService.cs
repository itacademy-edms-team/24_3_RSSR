using Microsoft.EntityFrameworkCore;
using RSSReader.Data;
using RSSReader.Models;

namespace RSSReader.Services
{
    public class FeedManagerService
    {
        private readonly AppDbContext _context;
        private readonly RssParser _parser;

        public FeedManagerService(AppDbContext context, RssParser parser)
        {
            _context = context;
            _parser = parser;
        }

        public async Task<List<Feed>> GetFeedsAsync()
        {
            var query = _context.Feeds
                .Include(f => f.FeedItems)
                .Include(f => f.Folders)
                .AsQueryable();

            return await query.ToListAsync();
        }

        public async Task<List<FeedItem>> GetFeedItemsAsync(int feedId)
        {
            return await _context.FeedItems
                .Where(x => x.FeedId == feedId)
                .OrderByDescending(x => x.PublishDate)
                .ToListAsync();
        }

        public async Task<bool> AddFeedAsync(string feedUrl)
        {
            if (!Uri.TryCreate(feedUrl, UriKind.Absolute, out _))
                return false;

            if (await _context.Feeds.AnyAsync(f => f.Url == feedUrl))
                return false;

            var parsedFeed = await _parser.ParseFeedAsync(feedUrl);
            var feed = new Feed
            {
                Url = feedUrl,
                Title = parsedFeed.Title,
                Description = parsedFeed.Description,
                ImageUrl = parsedFeed.ImageUrl,
                FeedItems = parsedFeed.FeedItems,
            };

            _context.Feeds.Add(feed);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task DeleteFeedAsync(int feedId)
        {
            var feed = await _context.Feeds.FindAsync(feedId);
            if (feed != null)
            {
                _context.Feeds.Remove(feed);
                await _context.SaveChangesAsync();
            }
        }

    }
}