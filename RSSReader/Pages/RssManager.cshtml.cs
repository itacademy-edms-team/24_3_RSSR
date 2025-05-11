using Microsoft.AspNetCore.Mvc.RazorPages;
using RSSReader.Data;
using RSSReader.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using RSSReader.Services;
public class RssManagerModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly RssParser _parser;
    public List<Feed> Feeds { get; set; } = new();
    public List<FeedItem> FeedItems { get; set; } = new();
    public int? SelectedFeedId { get; set; }

    public RssManagerModel(AppDbContext context, RssParser parser)
    {
        _context = context;
        _parser = parser;
    }

    public async Task OnGetAsync(int? feedId)
    {
        Feeds = await _context.Feeds.ToListAsync();
        SelectedFeedId = feedId;

        if (feedId.HasValue)
        {
            FeedItems = await _context.FeedItems
                .Where(x => x.FeedId == feedId.Value)
                .OrderByDescending(x => x.PublishDate)
                .ToListAsync();
        }
    }

    public async Task<IActionResult> OnPostAsync(string feedUrl)
    {
        if (!Uri.TryCreate(feedUrl, UriKind.Absolute, out _))
            return RedirectToPage();

        if (!await _context.Feeds.AnyAsync(f => f.Url == feedUrl))
        {
            var parseredFeed = await _parser.ParseFeedAsync(feedUrl);

            var feed = new Feed
            {
                Url = feedUrl,
                Title = parseredFeed.Title,
                Description = parseredFeed.Description,
                ImageUrl = parseredFeed.ImageUrl,
                FeedItems = parseredFeed.FeedItems,
            };
            _context.Feeds.Add(feed);
            await _context.SaveChangesAsync();
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteFeedAsync(int feedId)
    {
        var feed = await _context.Feeds.FindAsync(feedId);
        if (feed != null)
        {
            _context.Feeds.Remove(feed);
            await _context.SaveChangesAsync();
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnGetFeedItemsJsonAsync(int feedId)
    {
        var items = await _context.FeedItems
            .Where(f => f.FeedId == feedId)
            .OrderByDescending(f => f.PublishDate)
            .Select(f => new {
                title = f.Title,
                description = f.Description,
                link = f.Link
            })
            .ToListAsync();

        return new JsonResult(items);
    }

}
