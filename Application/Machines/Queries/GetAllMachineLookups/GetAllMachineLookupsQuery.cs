using System.Collections.Generic;
using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Machines.Queries.GetAllMachineLookups
{
    public class GetAllMachineLookupsQuery : IRequest<IEnumerable<MachineLookupDto>>
    {
    }
}
