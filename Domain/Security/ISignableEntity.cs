namespace AccountManager.Domain.Security
{
    public interface ISignableEntity
    {
        byte[] ToBytesData();
    }
}
