using System.Security.Cryptography;

namespace AccountManager.Common.Keys
{
    public class AsymmetricKeyPairFactory : IAsymmetricKeyPairFactory
    {
        public AsymmetricKeyPair Generate()
        {
            using (var dsa = DSA.Create())
            {
                var xmpPublicKey = dsa.ToXmlString(false);
                var xmlPrivateKey = dsa.ToXmlString(true);
                return new AsymmetricKeyPair
                {
                    PublicKey = xmpPublicKey,
                    PrivateKey = xmlPrivateKey
                };
            }
        }
    }

    public interface IAsymmetricKeyPairFactory
    {
        AsymmetricKeyPair Generate();
    }
}