namespace AccountManager.Domain
{
    public interface IAccountManagerMembershipUser
    {
        string Name { get; set; }
        object ProviderUserKey { get; }
        string UserName { get; }
        string[] Permissions { get; set; }
    }
}