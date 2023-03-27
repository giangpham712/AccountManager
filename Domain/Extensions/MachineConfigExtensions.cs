using System.Linq;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AccountManager.Domain.Entities.Machine;

namespace AccountManager.Domain.Extensions
{
    public static class MachineConfigExtensions
    {
        public static bool MatchVersions(this MachineConfig config, MachineConfig otherConfig)
        {
            return new VersionInfo(config.LauncherVersion).Version ==
                   new VersionInfo(otherConfig.LauncherVersion).Version &&
                   new VersionInfo(config.ReportingVersion).Version ==
                   new VersionInfo(otherConfig.ReportingVersion).Version &&
                   new VersionInfo(config.PdfExportVersion).Version ==
                   new VersionInfo(otherConfig.PdfExportVersion).Version &&
                   new VersionInfo(config.SiteMasterVersion).Version ==
                   new VersionInfo(otherConfig.SiteMasterVersion).Version &&
                   new VersionInfo(config.ClientVersion).Version ==
                   new VersionInfo(otherConfig.ClientVersion).Version &&
                   new VersionInfo(config.SqlExportVersion).Version ==
                   new VersionInfo(otherConfig.SqlExportVersion).Version &&
                   new VersionInfo(config.PopulateVersion).Version ==
                   new VersionInfo(otherConfig.PopulateVersion).Version &&
                   new VersionInfo(config.LinkwareVersion).Version ==
                   new VersionInfo(otherConfig.LinkwareVersion).Version &&
                   new VersionInfo(config.SmchkVersion).Version == new VersionInfo(otherConfig.SmchkVersion).Version &&
                   new VersionInfo(config.DiscoveryVersion).Version ==
                   new VersionInfo(otherConfig.DiscoveryVersion).Version &&
                   new VersionInfo(config.FiberSenSysVersion).Version ==
                   new VersionInfo(otherConfig.FiberSenSysVersion).Version &&
                   new VersionInfo(config.FiberMountainVersion).Version ==
                   new VersionInfo(otherConfig.FiberMountainVersion).Version &&
                   new VersionInfo(config.ServiceNowVersion).Version ==
                   new VersionInfo(otherConfig.ServiceNowVersion).Version &&
                   new VersionInfo(config.CommScopeVersion).Version ==
                   new VersionInfo(otherConfig.CommScopeVersion).Version;
        }

        public static bool MatchVersions(this MachineConfig config, Machine machine)
        {
            var state = machine.States.FirstOrDefault();
            if (state == null)
                return false;

            return (!machine.IsLauncher ||
                    new VersionInfo(config.LauncherVersion).Version == state.Launcher &&
                    new VersionInfo(config.PdfExportVersion).Version == state.PdfExport &&
                    new VersionInfo(config.ReportingVersion).Version == state.Reporting) &&
                   (!machine.IsSiteMaster ||
                    new VersionInfo(config.SiteMasterVersion).Version == state.SiteMaster &&
                    new VersionInfo(config.ClientVersion).Version == state.Client &&
                    new VersionInfo(config.SmchkVersion).Version == state.Smchk &&
                    new VersionInfo(config.LinkwareVersion).Version == state.Linkware &&
                    new VersionInfo(config.DiscoveryVersion).Version == state.Discovery &&
                    new VersionInfo(config.SqlExportVersion).Version == state.SqlExport) &&
                   new VersionInfo(config.PopulateVersion).Version == state.Populate &&
                   new VersionInfo(config.FiberSenSysVersion).Version == state.FiberSenSys &&
                   new VersionInfo(config.FiberMountainVersion).Version == state.FiberMountain &&
                    new VersionInfo(config.ServiceNowVersion).Version == state.ServiceNow &&
                   new VersionInfo(config.CommScopeVersion).Version == state.CommScope;
        }
    }
}