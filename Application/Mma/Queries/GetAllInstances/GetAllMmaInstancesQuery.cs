using System.Collections.Generic;
using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Mma.Queries.GetAllInstances
{
    public class GetAllMmaInstancesQuery : IRequest<IEnumerable<MmaInstanceDto>>
    {
    }
}