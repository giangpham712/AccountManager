using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace AccountManager.Domain.Security.Licensing
{
    [Serializable]
    public class IrmLicense : ISignableEntity
    {
        // We updated to use XmlSerializer instead to easier deal with changes in the structure of class IrmLicense
        // Before that we used BinaryFormatter
        // To keep the serialization and license verification backward compatible, newly added fields are marked with NonSerialized

        [NonSerialized]
        private int? _maxClientUsers;
        [NonSerialized]
        private int? _maxReportUsers;
        [NonSerialized]
        private int? _maxTechnicianUsers;

        public IrmLicense()
        {
            Id = Guid.NewGuid();
        }

        [JsonIgnore]
        public Guid Id { get; set; }

        public int MaxSiteCount { get; set; }

        public int MaxAreaCount { get; set; }

        public int MaxEquipCount { get; set; }

        public int MaxSoftwareCount { get; set; }

        public int MaxCircuitCount { get; set; }

        public int MaxPathwayCount { get; set; }

        public int MaxMainHoleCount { get; set; }

        public int MaxUserCount { get; set; }

        public int MaxFaceplateCount { get; set; }

        public int MaxRackCount { get; set; }

        public int? MaxCableCount { get; set; }

        public int? MaxContactCount { get; set; }

        public int MaxCloudInstanceCredits { get; set; }

        public int? MaxClientUsers
        {
            get { return _maxClientUsers; }
            set { _maxClientUsers = value; }
        }

        public int? MaxReportUsers
        {
            get { return _maxReportUsers; }
            set { _maxReportUsers = value; }
        }

        public int? MaxTechnicianUsers
        {
            get { return _maxTechnicianUsers; }
            set { _maxTechnicianUsers = value; }
        }

        public bool IsActive { get; set; }

        public DateTime Expiration { get; set; }

        public string[] Features { get; set; }

        public string[] ReportCategories { get; set; }

        public string AddOnSiteName { get; set; }

        public string AddOnAreaCategoryName { get; set; }

        public IrmLicense[] AddOnLicenses { get; set; }

        public byte[] ToBytesData()
        {
            var json = JsonConvert.SerializeObject(this, GetSerializationSettings());
            return Encoding.UTF8.GetBytes(json);
        }

        public static class Feature
        {
            public const string RackElevation = "RackElevation";
            public const string Butterfly = "Butterfly";
            public const string Reporting = "Reporting";
            public const string UserFieldSpecs = "UserFieldSpecs";
            public const string MAC = "MAC";
            public const string Circuit = "Circuit";
            public const string UserBulkImport = "UserBulkImport";
            public const string CustomizeLogo = "CustomizeLogo";
            public const string RelationalExport = "RelationalExport";
            public const string Linkware = "Linkware";
            public const string Remedy = "Remedy";
            public const string BMC = "BMC";
            public const string FiberSenSys = "FiberSenSys";
            public const string osTicket = "osTicket";
        }

        public static JsonSerializerSettings GetSerializationSettings()
        {
            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
                DateFormatString = "yyyy-MM-ddTH:mm:ss.fffZ"
            };

            settings.Converters.Add(new StringEnumConverter());

            return settings;
        }
    }
}
