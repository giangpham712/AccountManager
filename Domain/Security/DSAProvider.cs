using System;
using System.IO;
using System.Security.Cryptography;

namespace AccountManager.Domain.Security
{
    public static class DSAProvider    
    {
        public static byte[] Sign(byte[] data, string privateKey)
        {
            using (var provider = DSA.Create())
            {
                using (var sha = SHA1.Create())
                {
                    var rgb = sha.ComputeHash(data);
                    provider.FromXmlString(privateKey);
                    var signature = provider.CreateSignature(rgb);
                    return signature;
                }
            }
        }

        public static SignedEntity<TEntity> Sign<TEntity>(TEntity entity, string privateKey)
            where TEntity : ISignableEntity, new()
        {
            var signature = Sign(entity.ToBytesData(), privateKey);
            return new SignedEntity<TEntity>()
            {
                Entity = entity,
                Signature = signature
            };
        }

        public static bool Verify(byte[] data, byte[] signature, string publicKey)
        {
            using (var provider = DSA.Create())
            {
                using (var sha = SHA1.Create())
                {
                    var rgb = sha.ComputeHash(data);
                    provider.FromXmlString(publicKey);
                    return provider.VerifySignature(rgb, signature);                        
                }
            }
        }

        public static bool Verify<TEntity>(SignedEntity<TEntity> signedEntity, Func<TEntity, byte[]> getBytesFunc, string publicKey)
            where TEntity : ISignableEntity, new()
        {
            var data = getBytesFunc(signedEntity.Entity);
            return Verify(data, signedEntity.Signature, publicKey);
        }

        public static bool Verify<TEntity>(SignedEntity<TEntity> signedEntity, string publicKey)
            where TEntity: ISignableEntity, new()
        {
            var data = signedEntity.Entity.ToBytesData();
            return Verify(data, signedEntity.Signature, publicKey);
        }

        public static void GenerateKeyPair(string name)
        {
            var publicPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, name + ".public");
            var privatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, name + ".private");

            using (var provider = DSA.Create())
            {
                var publicKey = provider.ToXmlString(false);
                var privateKey = provider.ToXmlString(true);

                using (var writer = new StreamWriter(publicPath))
                {
                    writer.Write(publicKey);
                    writer.Flush();
                    writer.Close();
                }

                using (var writer = new StreamWriter(privatePath))
                {
                    writer.Write(privateKey);
                    writer.Flush();
                    writer.Close();
                }
            }
        }
    }
}
