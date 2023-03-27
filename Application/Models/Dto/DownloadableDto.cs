namespace AccountManager.Application.Models.Dto
{
    public class DownloadableDto
    {
        public byte[] Content { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
    }
}