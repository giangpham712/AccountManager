namespace AccountManager.Application.Saas.Commands.DeleteSite
{
    public class DeleteSiteCommand : CommandBase
    {
        public string UrlFriendlyName { get; set; }
        public string AccountUrlFriendlyName { get; set; }
    }
}