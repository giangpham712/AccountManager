namespace AccountManager.Application.Models.Dto
{
    public class SiteStatusDto
    {
        public long Id { get; set; }
        public string InstanceStatusDetail { get; set; }
        public string InstanceStatus { get; set; }
        public string SiteStatus { get; set; }
        public bool NeedsAdmin { get; set; }
    }
}