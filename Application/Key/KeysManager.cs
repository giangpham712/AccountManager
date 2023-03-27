using System;
using System.IO;

namespace AccountManager.Application.Key
{
    public class KeysManager : IKeysManager
    {
        private string _defaultInterServerPrivateKey;
        private string _defaultInterServerPublicKey;
        private string _defaultLicensePrivateKey;
        private string _defaultLicensePublicKey;
        private string _defaultUserTokenPrivateKey;
        private string _defaultUserTokenPublicKey;

        private string _lctInterServerPublicKey;

        public string DefaultUserTokenPublicKey =>
            _defaultUserTokenPublicKey ?? (_defaultUserTokenPublicKey = GetDefaultKey("UserTokenKey.public"));

        public string DefaultUserTokenPrivateKey =>
            _defaultUserTokenPrivateKey ?? (_defaultUserTokenPrivateKey = GetDefaultKey("UserTokenKey.private"));

        public string DefaultInterServerPublicKey =>
            _defaultInterServerPublicKey ?? (_defaultInterServerPublicKey = GetDefaultKey("InterServerKey.public"));

        public string DefaultInterServerPrivateKey =>
            _defaultInterServerPrivateKey ?? (_defaultInterServerPrivateKey = GetDefaultKey("InterServerKey.private"));

        public string DefaultLicensePublicKey =>
            _defaultLicensePublicKey ?? (_defaultLicensePublicKey = GetDefaultKey("LicenseKey.public"));

        public string DefaultLicensePrivateKey =>
            _defaultLicensePrivateKey ?? (_defaultLicensePrivateKey = GetDefaultKey("LicenseKey.private"));

        public string LctInterServerPublicKey =>
            _lctInterServerPublicKey ?? (_lctInterServerPublicKey = GetDefaultKey("LctInterServerKey.public"));


        public AsymmetricKeyPair GenerateUserTokenKeyPair(bool useDefault = false)
        {
            if (!useDefault) return AsymmetricKeyUtils.Generate();

            return new AsymmetricKeyPair
            {
                PublicKey = DefaultUserTokenPublicKey,
                PrivateKey = DefaultUserTokenPrivateKey
            };
        }

        public AsymmetricKeyPair GenerateInterServerKeyPair(bool useDefault = false)
        {
            if (!useDefault) return AsymmetricKeyUtils.Generate();

            return new AsymmetricKeyPair
            {
                PublicKey = DefaultInterServerPublicKey,
                PrivateKey = DefaultInterServerPrivateKey
            };
        }

        public AsymmetricKeyPair GenerateLicenseKeyPair(bool useDefault = false)
        {
            if (!useDefault) return AsymmetricKeyUtils.Generate();

            return new AsymmetricKeyPair
            {
                PublicKey = DefaultLicensePublicKey,
                PrivateKey = DefaultLicensePrivateKey
            };
        }

        private static string GetDefaultKey(string keyFileName)
        {
            var keyFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Keys", keyFileName);
            var r = ReadFile(keyFilePath);
            return r;
        }

        private static string ReadFile(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException(string.Format("File {0} doesn't exist", path));

            using (var reader = new StreamReader(File.OpenRead(path)))
            {
                return reader.ReadToEnd();
            }
        }
    }

    public interface IKeysManager
    {
        string LctInterServerPublicKey { get; }

        AsymmetricKeyPair GenerateUserTokenKeyPair(bool useDefault = false);
        AsymmetricKeyPair GenerateInterServerKeyPair(bool useDefault = false);
        AsymmetricKeyPair GenerateLicenseKeyPair(bool useDefault = false);
    }
}