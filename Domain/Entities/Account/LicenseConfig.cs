using System;
using System.Collections.Generic;
using System.Linq;

namespace AccountManager.Domain.Entities.Account
{
    public class LicenseConfig : ISupportTemplate
    {
        private static Dictionary<string, string> ReportCatMappings = new Dictionary<string, string>()
        {
            { "Equip", "Equipment" },
            { "Equipment", "Equip" },
            { "Equipment Type", "EquipType" },
            { "EquipType", "Equipment Type" },
            { "CableType", "Cable Type" },
            { "Cable Type", "CableType" },
            { "CircuitType", "Circuit Type" },
            { "Circuit Type", "CircuitType" },
            { "PathwayType", "Pathway Type" },
            { "Pathway Type", "PathwayType" },
            { "MainHole", "Maintenance Hole" },
            { "Maintenance Hole", "MainHole" },
        };

        private string[] _reportingCategories;
        public ServerInstancePolicy InstancePolicy { get; set; }
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
        public int CloudCredits { get; set; }
        public string[] Features { get; set; }

        public string[] ReportingCategories
        {
            get => _reportingCategories;
            set
            {
                var reportingCategories = value.ToList();

                foreach (var entry in ReportCatMappings)
                {
                    if (reportingCategories.Contains(entry.Key))
                    {
                        reportingCategories.Add(entry.Value);
                    }
                }

                _reportingCategories = reportingCategories.Distinct().ToArray();
            }
        }

        public int? MaxReportUsers { get; set; }
        public int? MaxClientUsers { get; set; }
        public int? MaxTechnicianUsers { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset? ExpirationTime { get; set; }
        public string AddOnSiteName { get; set; }
        public string AddOnAreaCategoryName { get; set; }

        public string Creator { get; set; }

        public Account Account { get; set; }
        public long? AccountId { get; set; }

        public long Id { get; set; }
        public bool IsTemplate { get; set; }
        public bool IsPublic { get; set; }
        public string Name { get; set; }

        public bool Match(LicenseConfig other)
        {
            return MaxAreas == other.MaxAreas &&
                   MaxCables == other.MaxCables &&
                   MaxCircuits == other.MaxCircuits &&
                   MaxClientUsers == other.MaxClientUsers &&
                   MaxTechnicianUsers == other.MaxTechnicianUsers &&
                   MaxContacts == other.MaxContacts &&
                   MaxCables == other.MaxCables &&
                   MaxFaceplates == other.MaxFaceplates &&
                   MaxMainholes == other.MaxMainholes &&
                   MaxPathways == other.MaxPathways &&
                   MaxRacks == other.MaxRacks &&
                   MaxReportUsers == other.MaxReportUsers &&
                   MaxSites == other.MaxSites &&
                   MaxSoftwares == other.MaxSoftwares &&
                   MaxUsers == other.MaxUsers;
        }
    }
}