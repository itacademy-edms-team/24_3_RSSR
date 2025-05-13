using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RSSReader.Data;
using RSSReader.Models;

namespace RSSReader.Services
{
    /// <summary>
    /// Сервис для организации RSS-лент в папки.
    /// Основное назначение: категоризация лент пользователем.
    /// 
    /// Функции:
    /// - Создание/удаление папок
    /// - Назначение лент папкам
    /// - Обновление состава папок
    public class FolderService
    {
        private readonly AppDbContext _context;

        public FolderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddFolderAsync(string NewFolderName, List<int> SelectedFeedIds)
        {

            var folder = new Folder { FolderName = NewFolderName };

            if (SelectedFeedIds != null && SelectedFeedIds.Any())
            {
                folder.Feeds = await _context.Feeds
                    .Where(f => SelectedFeedIds.Contains(f.FeedId))
                    .ToListAsync();
            }

            _context.Folders.Add(folder);
            await _context.SaveChangesAsync();

        }

        public async Task<List<Folder>> GetAllFoldersWithFeedsAsync()
        {
            return await _context.Folders.Include(f => f.Feeds).ToListAsync();
        }

        public async Task<List<Feed>> GetAllFeedsAsync()
        {
            return await _context.Feeds.ToListAsync();
        }

        public async Task DeleteFolderAsync(int id)
        {
            var folder = await _context.Folders.FindAsync(id);
            if (folder != null)
            {
                _context.Folders.Remove(folder);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<Folder?> GetFolderWithFeedsAsync(int folderId)
        {
            return await _context.Folders
                .Include(f => f.Feeds)
                .FirstOrDefaultAsync(f => f.FolderId == folderId);
        }

        public async Task UpdateFolderFeedsAsync(int folderId, List<int> selectedFeedIds)
        {
            var folder = await _context.Folders
                .Include(f => f.Feeds)
                .FirstOrDefaultAsync(f => f.FolderId == folderId);

            if (folder == null) return;

            folder.Feeds.Clear();

            if (selectedFeedIds != null && selectedFeedIds.Any())
            {
                var selectedFeeds = await _context.Feeds
                    .Where(f => selectedFeedIds.Contains(f.FeedId))
                    .ToListAsync();

                foreach (var feed in selectedFeeds)
                    folder.Feeds.Add(feed);
            }

            await _context.SaveChangesAsync();
        }
    }
}
