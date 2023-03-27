﻿using System;
using System.Collections.Generic;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;

namespace AccountManager.Application.Accounts.Commands.CreateAccount
{
    public class CreateAccountCommand : CommandBase<long>
    {
        public string Name { get; set; }
        public string UrlFriendlyName { get; set; }
        public string Description { get; set; }

        public string SiteMasterName { get; set; }
        public string SiteMasterUrlFriendlyName { get; set; }
        public int SiteMasterPort { get; set; }

        public string Customer { get; set; }
        public long? ClassId { get; set; }
        public long? ParentId { get; set; }
        public bool Managed { get; set; }
        public bool AutoTest { get; set; }
        public string AutoTestBranch { get; set; }
        public bool WhiteGlove { get; set; }

        #region Idle schedules

        public IEnumerable<IdleScheduleDto> IdleSchedules { get; set; }

        #endregion

        #region Contact

        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone1 { get; set; }
        public string ContactPhone2 { get; set; }

        #endregion

        #region Billing

        public BillingPeriod BillingPeriod { get; set; }
        public double BillingAmount { get; set; }

        #endregion

        #region License

        public ServerInstancePolicy InstancePolicy { get; set; }
        public long CloudInstanceType { get; set; }
        public long? RedisCloudInstanceType { get; set; }
        public int MaxSites { get; set; }
        public int MaxContacts { get; set; }
        public int MaxCables { get; set; }
        public int MaxAreas { get; set; }
        public int MaxEquips { get; set; }
        public int MaxSoftwares { get; set; }
        public int MaxCircuits { get; set; }
        public int MaxPathways { get; set; }
        public int MaxMainholes { get; set; }
        public int MaxUsers { get; set; }
        public int MaxFaceplates { get; set; }
        public int MaxRacks { get; set; }
        public int? MaxReportUsers { get; set; }
        public int? MaxClientUsers { get; set; }
        public int? MaxTechnicianUsers { get; set; }
        public int CloudCredits { get; set; }
        public string[] Features { get; set; }
        public string[] ReportingCategories { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset? ExpirationTime { get; set; }
        public string AddOnSiteName { get; set; }
        public string AddOnAreaCategoryName { get; set; }

        #endregion

        #region Backup

        public int DailyDaysToRetain { get; set; }
        public string DailyEolAction { get; set; }

        public int WeeklyDaysToRetain { get; set; }
        public int WeeklyBackupDay { get; set; }
        public string WeeklyEolAction { get; set; }
        public string MonthlyEolAction { get; set; }
        public int MonthlyBackupDay { get; set; }
        public int MonthlyDaysToRetain { get; set; }

        public DateTimeOffset[] Times { get; set; }

        #endregion

        #region Instance settings

        public string Region { get; set; }
        public bool ShowInGrafana { get; set; }
        public bool UseSparePool { get; set; }
        public bool IncludeSampleData { get; set; }
        public string SampleDataFile { get; set; }
        public bool IncludeIrmoData { get; set; }
        public KeyType KeyType { get; set; }
        public bool EnableSsl { get; set; }
        public bool RunSmokeTest { get; set; }
        public string VmImageName { get; set; }
        public DateTimeOffset? AutoStopTime { get; set; }

        #region Software versions

        public string LauncherVersionMode { get; set; }
        public string LauncherVersion { get; set; }

        public string SiteMasterVersionMode { get; set; }
        public string SiteMasterVersion { get; set; }

        public string ClientVersionMode { get; set; }
        public string ClientVersion { get; set; }

        public string ReportingVersionMode { get; set; }
        public string ReportingVersion { get; set; }

        public string PdfExportVersionMode { get; set; }
        public string PdfExportVersion { get; set; }

        public string SqlExportVersionMode { get; set; }
        public string SqlExportVersion { get; set; }

        public string PopulateVersionMode { get; set; }
        public string PopulateVersion { get; set; }

        public string DeployerVersionMode { get; set; }
        public string DeployerVersion { get; set; }

        public string LinkwareVersionMode { get; set; }
        public string LinkwareVersion { get; set; }

        public string SmchkVersionMode { get; set; }
        public string SmchkVersion { get; set; }

        public string DiscoveryVersionMode { get; set; }
        public string DiscoveryVersion { get; set; }

        public string FiberSenSysVersionMode { get; set; }
        public string FiberSenSysVersion { get; set; }

        public string FiberMountainVersionMode { get; set; }
        public string FiberMountainVersion { get; set; }

        public string ServiceNowVersionMode { get; set; }
        public string ServiceNowVersion { get; set; }

        public string CommScopeVersionMode { get; set; }
        public string CommScopeVersion { get; set; }

        #endregion

        public string MainLibraryMode { get; set; }
        public long[] MainLibraryFiles { get; set; }

        public string AccountLibraryMode { get; set; }
        public long? AccountLibraryFile { get; set; }

        #endregion
    }
}