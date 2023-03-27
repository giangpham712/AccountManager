using System.Security.Cryptography;

namespace AccountManager.Application.Key
{
    public static class AsymmetricKeyUtils
    {
        public static AsymmetricKeyPair Generate()
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
}