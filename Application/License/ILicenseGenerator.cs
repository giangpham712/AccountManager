using System.Collections.Generic;
using AccountManager.Domain.Entities.Account;

namespace AccountManager.Application.License
{
    public interface ILicenseGenerator
    {
        byte[] GenerateLicense(LicenseConfig licenseConfig, IEnumerable<LicenseConfig> addOnLicenseConfigs, string licensePrivateKey);
    }
}