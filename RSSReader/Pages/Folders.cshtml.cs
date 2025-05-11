using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RSSReader.Data;
using RSSReader.Models;
using Microsoft.EntityFrameworkCore;

public class FoldersModel : PageModel
{
    private readonly AppDbContext _context;

    public FoldersModel(AppDbContext context)
    {
        _context = context;
    }

    public List<Folder> Folders { get; set; } = new();

    [BindProperty]
    public string NewFolderName { get; set; }

    [BindProperty]
    public List<int> SelectedFeedIds { get; set; } = new();

    public List<Feed> AllFeeds { get; set; } = new();

    public async Task OnGetAsync()
    {
        Folders = await _context.Folders.Include(f => f.Feeds).ToListAsync();
        AllFeeds = await _context.Feeds.ToListAsync();
    }

    public async Task<IActionResult> OnPostAddAsync()
    {
        if (string.IsNullOrWhiteSpace(NewFolderName))
        {
            ModelState.AddModelError("NewFolderName", "Название папки обязательно.");
            await OnGetAsync();
            return Page();
        }

        var folder = new Folder { FolderName = NewFolderName };

        if (SelectedFeedIds != null && SelectedFeedIds.Any())
        {
            folder.Feeds = await _context.Feeds
                .Where(f => SelectedFeedIds.Contains(f.FeedId))
                .ToListAsync();
        }

        _context.Folders.Add(folder);
        await _context.SaveChangesAsync();

        return RedirectToPage();
    }
    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var folder = await _context.Folders.FindAsync(id);
        if (folder != null)
        {
            _context.Folders.Remove(folder);
            await _context.SaveChangesAsync();
        }
        return RedirectToPage();
    }
}
