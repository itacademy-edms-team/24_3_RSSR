namespace RSSReader.Models
{
    public class Folder
    {
        public int FolderId { get; set; }
        public string FolderName { get; set; }

        public ICollection<Feed> Feeds { get; set; }

    }
}
