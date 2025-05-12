using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using RSSReader.Models;
using RSSReader.Services;

public class FoldersModel : PageModel
{
    private readonly FolderService _folderService;

    public FoldersModel(FolderService folderService)
    {
        _folderService = folderService;
    }

    public List<Folder> Folders { get; set; } = new();

    [BindProperty]
    public string NewFolderName { get; set; }

    [BindProperty]
    public List<int> SelectedFeedIds { get; set; } = new();

    public List<Feed> AllFeeds { get; set; } = new();

    public async Task OnGetAsync()
    {
        Folders = await _folderService.GetAllFoldersWithFeedsAsync();
        AllFeeds = await _folderService.GetAllFeedsAsync();
    }

    public async Task<IActionResult> OnPostAddAsync()
    {
        if (string.IsNullOrWhiteSpace(NewFolderName))
        {
            ModelState.AddModelError("NewFolderName", "Название папки обязательно.");
            await OnGetAsync();
            return Page();
        }

        await _folderService.AddFolderAsync(NewFolderName, SelectedFeedIds);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _folderService.DeleteFolderAsync(id);
        return RedirectToPage();
    }
}