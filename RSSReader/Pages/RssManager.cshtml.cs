using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using RSSReader.Models;
using RSSReader.Services;

public class RssManagerModel : PageModel
{
    private readonly FeedManagerService _feedManager;

    public List<Feed> Feeds { get; set; } = new();
    public List<FeedItem> FeedItems { get; set; } = new();
    public int? SelectedFeedId { get; set; }

    public RssManagerModel(FeedManagerService feedManager)
    {
        _feedManager = feedManager;
    }

    public async Task OnGetAsync(int? feedId)
    {
        Feeds = await _feedManager.GetFeedsAsync();
        SelectedFeedId = feedId;

        if (feedId.HasValue)
            FeedItems = await _feedManager.GetFeedItemsAsync(feedId.Value);
    }

    public async Task<IActionResult> OnPostAsync(string feedUrl)
    {
        if (await _feedManager.AddFeedAsync(feedUrl))
            return RedirectToPage();

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteFeedAsync(int feedId)
    {
        await _feedManager.DeleteFeedAsync(feedId);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnGetFeedItemsJsonAsync(int feedId)
    {
        var items = await _feedManager.GetFeedItemsAsync(feedId);
        return new JsonResult(items.Select(f => new {
            title = f.Title,
            description = f.Description,
            link = f.Link
        }));
    }
}