using System.Collections.Generic;
using System.Threading.Tasks;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Machine;

namespace AccountManager.Application.Configs
{
    public interface IConfigGenerator
    {
        Task<Dictionary<string, object>> GenerateComponentConfig(Machine machine, Site site);

        Task<Dictionary<string, object>> GenerateDeployerConfig(Machine machine, Site site);
    }
}