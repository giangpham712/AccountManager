using System;

namespace AccountManager.Application.Accounts.Commands.UpdateLicenseSettings
{
    public class UpdateLicenseSettingsCommand : CommandBase
    {
        public long AccountId { get; set; }

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
        public int CloudCredits { get; set; }
        public int? MaxReportUsers { get; set; }
        public int? MaxClientUsers { get; set; }
        public int? MaxTechnicianUsers { get; set; }
        public string[] Features { get; set; }
        public string[] ReportingCategories { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset? ExpirationTime { get; set; }
        public string AddOnSiteName { get; set; }
        public string AddOnAreaCategoryName { get; set; }
    }
}