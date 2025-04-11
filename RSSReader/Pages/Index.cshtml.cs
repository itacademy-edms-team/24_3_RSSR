using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using RSSReader.Models;
using RSSReader.Services;

public class IndexModel : PageModel
{
    private readonly RssParser _parser;
    private readonly FeedStorage _storage;

    public List<FeedItem> FeedItems { get; set; } = new();
    public List<string> FeedUrls { get; set; } = new();

    public IndexModel(RssParser parser, FeedStorage storage)
    {
        _parser = parser;
        _storage = storage;
    }

    public void OnGet(string searchQuery)
    {
       
        FeedUrls = _storage.LoadFeeds();
        FeedItems = FeedUrls
            .Distinct() 
            .SelectMany(url => _parser.ParseFeed(url))
            .GroupBy(x => x.Link) 
            .Select(g => g.First()) 
            .OrderByDescending(i => i.PublishDate)
            .ToList();

        //Логика поиска
        if (!string.IsNullOrEmpty(searchQuery))
        {
            FeedItems = FeedItems
                .Where(i => i.Title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                           i.Description.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }


    public IActionResult OnPost(string feedUrl)
    {
        if (Uri.TryCreate(feedUrl, UriKind.Absolute, out _))
        {
            var feeds = _storage.LoadFeeds();
            feeds.Add(feedUrl);
            _storage.SaveFeeds(feeds);
        }
        return RedirectToPage();
    }
}