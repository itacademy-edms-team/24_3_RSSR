using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RSSReader.Data;
using RSSReader.Models;

public class ManageFeedsInFolderModel : PageModel
{
    private readonly AppDbContext _context;
    public ManageFeedsInFolderModel(AppDbContext context) 
    {
        _context = context;
    }

    [BindProperty]
    public Folder Folder { get; set; }
    public List<Feed> AllFeeds { get; set; }

    [BindProperty]
    public List<int> SelectedFeedIds { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int folderId)
    {
        Folder = await _context.Folders
            .Include(f => f.Feeds)
            .FirstOrDefaultAsync(f => f.FolderId == folderId);

        if (Folder == null) return NotFound();

        AllFeeds = await _context.Feeds.ToListAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int folderId)
    {
        Folder = await _context.Folders
            .Include(f => f.Feeds)
            .FirstOrDefaultAsync(f => f.FolderId == folderId);

        if (Folder == null) return NotFound();

        AllFeeds = await _context.Feeds.ToListAsync();


        Folder.Feeds.Clear();
        if (SelectedFeedIds != null && SelectedFeedIds.Any())
        {
            var selectedFeeds = await _context.Feeds
                .Where(f => SelectedFeedIds.Contains(f.FeedId))
                .ToListAsync();

            foreach (var feed in selectedFeeds)
                Folder.Feeds.Add(feed);
        }

        await _context.SaveChangesAsync();
        TempData["Message"] = "Список фидов обновлен!";
        return RedirectToPage(new { folderId });
    }
}
