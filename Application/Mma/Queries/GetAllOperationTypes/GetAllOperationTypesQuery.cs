using System.Collections.Generic;
using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Mma.Queries.GetAllOperationTypes
{
    public class GetAllOperationTypesQuery : IRequest<IEnumerable<OperationTypeDto>>
    {
    }
}