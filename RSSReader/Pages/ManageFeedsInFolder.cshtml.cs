using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RSSReader.Models;
using RSSReader.Services;

public class ManageFeedsInFolderModel : PageModel
{
    private readonly FolderService _folderService;

    public ManageFeedsInFolderModel(FolderService folderService)
    {
        _folderService = folderService;
    }

    [BindProperty]
    public Folder Folder { get; set; }
    public List<Feed> AllFeeds { get; set; }

    [BindProperty]
    public List<int> SelectedFeedIds { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int folderId)
    {
        Folder = await _folderService.GetFolderWithFeedsAsync(folderId);
        if (Folder == null) return NotFound();

        AllFeeds = await _folderService.GetAllFeedsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int folderId)
    {
        await _folderService.UpdateFolderFeedsAsync(folderId, SelectedFeedIds);
        TempData["Message"] = "Список фидов обновлен!";
        return RedirectToPage(new { folderId });
    }
}