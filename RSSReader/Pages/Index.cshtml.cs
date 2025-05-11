using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RSSReader.Data;
using RSSReader.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using RSSReader.Services;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly RssParser _parser;

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

    public IndexModel(AppDbContext context, RssParser parser)
    {
        _context = context;
        _parser = parser;
    }

    public async Task OnGetAsync()
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

        var query = _context.FeedItems
            .Include(i => i.Feed)
            .AsQueryable();

        ApplyFilters(ref query);
        ApplySorting(ref query);

        FeedItems = await query.ToListAsync();
        BuildActiveFiltersList();
    }

    private void ApplyFilters(ref IQueryable<FeedItem> query)
    {

        if (HasImages)
        {
            query = query.Where(i => i.Description != null &&
               (i.Description.Contains("<img") ||
                i.Description.Contains("<image") ||
                i.Description.Contains("<picture")));
        }

        if (WithoutImages)
        {
            query = query.Where(i => i.Description != null &&
               !(i.Description.Contains("<img") ||
                i.Description.Contains("<image") ||
                i.Description.Contains("<picture")));
        }

        if (LastDays.HasValue)
            query = query.Where(i => i.PublishDate >= DateTime.Now.AddDays(-LastDays.Value));

        if (!string.IsNullOrEmpty(SearchQuery))
            query = query.Where(i =>
                EF.Functions.Like(i.Title, $"%{SearchQuery}%") ||
                EF.Functions.Like(i.Description, $"%{SearchQuery}%"));
    }

    private void ApplySorting(ref IQueryable<FeedItem> query)
    {
        switch (SortOrder)
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

    private void BuildActiveFiltersList()
    {

        if (HasImages)
            ActiveFilters.Add("С изображениями");

        if (WithoutImages)
            ActiveFilters.Add("Без изображений");

        if (LastDays.HasValue)
            ActiveFilters.Add($"Последние {LastDays} дней");

        if (!string.IsNullOrEmpty(SearchQuery))
            ActiveFilters.Add($"Поиск: '{SearchQuery}'");
    }

}