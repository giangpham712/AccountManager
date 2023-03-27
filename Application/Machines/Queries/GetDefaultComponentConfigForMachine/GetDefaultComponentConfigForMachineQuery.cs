using AccountManager.Application.Models.Dto;

namespace AccountManager.Application.Machines.Queries.GetDefaultComponentConfigForMachine
{
    public class GetDefaultComponentConfigForMachineQuery : CommandBase<ConfigDto>
    {
        public long Id { get; set; }
    }
}
