using System;

namespace AccountManager.Application.Models.Dto
{
    public class LicenseConfigDto
    {
        public long Id { get; set; }
        public string InstancePolicy { get; set; }
        public long CloudInstanceType { get; set; }
        public long? RedisCloudInstanceType { get; set; }
        public int MaxSites { get; set; }
        public int MaxAreas { get; set; }
        public int MaxEquips { get; set; }
        public int MaxContacts { get; set; }
        public int MaxCables { get; set; }
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

        public string Creator { get; set; }
        public bool IsPublic { get; set; }
        public bool IsTemplate { get; set; }
        public string Name { get; set; }

        public string MatchedTemplate { get; set; }
    }
}