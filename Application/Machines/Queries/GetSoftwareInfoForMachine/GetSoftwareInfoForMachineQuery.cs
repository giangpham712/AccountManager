using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Machines.Queries.GetSoftwareInfoForMachine
{
    public class GetSoftwareInfoForMachineQuery : IRequest<MachineStateInfoDto>
    {
        public long Id { get; set; }
    }
}