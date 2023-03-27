using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AccountManager.Application.Utils;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AccountManager.Domain.Entities.Machine;
using AccountManager.Domain.Entities.Public;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Configs
{
    public class ConfigGenerator : IConfigGenerator
    {
        private readonly ICloudStateDbContext _context;

        public ConfigGenerator(ICloudStateDbContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<string, object>> GenerateComponentConfig(Machine machine, Site site)
        {
            var account = machine.Account;
            var componentConfigs = await _context.Set<ComponentConfig>().ToListAsync();

            var variables = GetVariables(site, machine.Account);

            var configs = componentConfigs.ToDictionary(x => $"{x.RootKey}.{x.SubKey}", x =>
            {
                var dataType = x.DataType;
                var defaultValue = x.SaasDefaultValue;

                return GenerateValue(dataType, defaultValue, variables);
            });

            configs["common.saasApiPassword_"] = account.Keys.ApiPassword;
            configs["common.interServerPublicKeyData_"] = account.Keys.InterServerPublic;
            configs["common.interServerPrivateKeyData_"] = account.Keys.InterServerPrivate;
            configs["common.userTokenPublicKeyData_"] = account.Keys.UserTokenPublic;

            configs["integration.cloudMachineCreds_"] = account.Keys.AccountFile;

            configs["common.sqlExportApiPassword_"] = account.Keys.SqlExportPass;
            
            configs["launcher.userTokenPrivateKeyData_"] = account.Keys.UserTokenPrivate;
            configs["launcher.licenseKeyPublicKeyData_"] = account.Keys.LicensePublic;
            configs["launcher.licenseFileData_"] = account.License;

            return configs;
        }

        public async Task<Dictionary<string, object>> GenerateDeployerConfig(Machine machine, Site site)
        {
            var deployerConfigs = await _context.Set<DeployerConfig>().ToListAsync();

            var variables = GetVariables(site, machine.Account);

            var configs = deployerConfigs.ToDictionary(x => $"{x.RootKey}.{x.SubKey}", x =>
            {
                var dataType = x.DataType;
                var defaultValue = x.DefaultValue;

                return GenerateValue(dataType, defaultValue, variables);
            });

            var mmaInstance = await _context.Set<MmaInstance>()
                .FirstOrDefaultAsync(x => x.MachineClassId == machine.ClassId);

            if (mmaInstance != null)
            {
                configs["general.caa_base_url"] = $"https://{mmaInstance.CaaHost}/irm/rest/v3t/";
            }

            return configs;
        }

        private static Dictionary<string, object> GetVariables(Site site, Account account)
        {
            var variables = new Dictionary<string, object>
            {
                { "protocol", account.MachineConfig.EnableSsl ? "https" : "http" },
                { "account_name", account.Name },
                { "account_url_name", account.UrlFriendlyName },
                { "site_name", site.Name },
                { "site_url_name", site.UrlFriendlyName },
                { "site_port", site.Port }
            };
            return variables;
        }

        private static object GenerateValue(string dataType, string defaultValue, Dictionary<string, object> variables)
        {
            object value;
            switch (dataType)
            {
                case "boolean":
                    bool.TryParse(defaultValue, out var boolValue);
                    value = (object)boolValue;
                    break;

                case "number":
                    if (long.TryParse(defaultValue, out var intValue))
                    {
                        value = intValue;
                    }
                    else if (decimal.TryParse(defaultValue, out var decimalValue))
                    {
                        value = decimalValue;
                    }
                    else
                    {
                        value = (object)default(int);
                    }

                    break;
                case null:
                    value = (object)ParseConfigExpression(defaultValue, variables);
                    break;

                default:
                    value = null;
                    break;
            }

            return value;
        }

        private static string ParseConfigExpression(string expression, Dictionary<string, object> variables)
        {
            if (expression == null)
            {
                return null;
            }

            try
            {
                return Regex.Replace(expression, @"\{(.+?)\}", m => variables[m.Groups[1].Value].ToString());
            }
            catch (Exception e)
            {
                return expression;
            }
        }
    }
}