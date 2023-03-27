using AccountManager.Application.Models.Dto;

namespace AccountManager.Application.Machines.Queries.GetDefaultDeployerConfigForMachine
{
    public class GetDefaultDeployerConfigForMachineQuery : CommandBase<ConfigDto>
    {
        public long Id { get; set; }
    }
}
