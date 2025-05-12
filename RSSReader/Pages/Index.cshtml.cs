using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RSSReader.Models;
using RSSReader.Services;

public class IndexModel : PageModel
{
    private readonly FeedItemService _feedItemsService;

    [BindProperty(SupportsGet = true)]
    public string SearchQuery { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool HasImages { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool WithoutImages { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? LastDays { get; set; }

    [BindProperty(SupportsGet = true)]
    public string SortOrder { get; set; } = "date_desc";

    public List<FeedItem> FeedItems { get; set; } = new();
    public List<string> ActiveFilters { get; set; } = new();
    public bool HasActiveFilters => ActiveFilters.Any();

    public IndexModel(FeedItemService feedItemsService)
    {
        _feedItemsService = feedItemsService;
    }

    public async Task OnGetAsync()
    {
        await _feedItemsService.UpdateFeedsAsync();

        FeedItems = await _feedItemsService.GetFeedItemsAsync(
            searchQuery: SearchQuery,
            hasImages: HasImages,
            withoutImages: WithoutImages,
            lastDays: LastDays,
            sortOrder: SortOrder
        );

        ActiveFilters = _feedItemsService.GetActiveFilters(
            searchQuery: SearchQuery,
            hasImages: HasImages,
            withoutImages: WithoutImages,
            lastDays: LastDays
        );
    }
}