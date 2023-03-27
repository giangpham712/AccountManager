using System;
using AccountManager.Domain.Entities;

namespace AccountManager.Application.Utils
{
    public static class AccountHelper
    {
        public static string GenerateMachineName(string accountUrlName, ServerInstancePolicy instancePolicy,
            string appName = null)
        {
            const string prefix = "MMA";
            switch (instancePolicy)
            {
                case ServerInstancePolicy.AllInOne:
                    return $"{prefix} {accountUrlName} AllInOne";
                case ServerInstancePolicy.InstancePerSiteMaster:
                    return $"{prefix} {accountUrlName} IPSM {appName}";
                default:
                    throw new ArgumentOutOfRangeException(nameof(instancePolicy), instancePolicy, null);
            }
        }

        public static string GenerateLauncherDomain(string accountUrlName)
        {
            return
                $"{accountUrlName}.planetassociates.net";
        }

        public static string GenerateLauncherUrl(string accountUrlName, bool enableSsl)
        {
            return
                $"{(enableSsl ? "https" : "http")}://{GenerateLauncherDomain(accountUrlName)}/";
        }

        public static string GenerateSiteMasterDomain(string accountUrlName, string siteUrlName)
        {
            return
                $"{siteUrlName}.{accountUrlName}.planetassociates.net";
        }

        public static string GenerateSiteMasterUrl(string accountUrlName, string siteUrlName, bool enableSsl)
        {
            return
                $"{(enableSsl ? "https" : "http")}://{GenerateSiteMasterDomain(accountUrlName, siteUrlName)}";
        }

        public static string GenerateSiteMasterApiUrl(string accountUrlName, string siteUrlName, bool enableSsl)
        {
            return
                $"{(enableSsl ? "https" : "http")}://{GenerateSiteMasterDomain(accountUrlName, siteUrlName)}:8080/irm/rest/v1.14/";
        }
    }
}