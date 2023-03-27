namespace AccountManager.Application.Models.Dto
{
    public class CreatableMachineDto
    {
        public string Name { get; set; }
        public bool IsLauncher { get; set; }
        public bool IsSiteMaster { get; set; }
        public long? SiteId { get; set; }
        public string SiteName { get; set; }
    }
}