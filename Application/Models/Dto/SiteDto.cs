namespace AccountManager.Application.Models.Dto
{
    public class SiteDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string UrlFriendlyName { get; set; }
        public int CloudInstanceType { get; set; }
        public int Port { get; set; }
    }
}