using System.Collections.Generic;
using System.Linq;
using System.Text;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AccountManager.Domain.Security;
using AccountManager.Domain.Security.Licensing;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace AccountManager.Application.License
{
    public class LicenseGenerator : ILicenseGenerator
    {
        private readonly IMapper _mapper;

        public LicenseGenerator(IMapper mapper)
        {
            _mapper = mapper;
        }

        public byte[] GenerateLicense(LicenseConfig licenseConfig,
            IEnumerable<LicenseConfig> addOnLicenseConfigs, string licensePrivateKey)
        {
            var license = _mapper.Map<IrmLicense>(licenseConfig);
            license.AddOnLicenses = addOnLicenseConfigs?.Select(x => { return _mapper.Map<IrmLicense>(x); }).ToArray();

            var signedLicense = DSAProvider.Sign(license, licensePrivateKey);
            var serializationSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
                DateFormatString = "yyyy-MM-ddTH:mm:ss.fffZ"
            };

            serializationSettings.Converters.Add(new StringEnumConverter());

            var jsonStr = JsonConvert.SerializeObject(signedLicense, serializationSettings);
            var bytes = Encoding.UTF8.GetBytes(jsonStr);

            return bytes;
        }
    }
}