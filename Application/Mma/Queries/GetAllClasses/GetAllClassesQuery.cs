using System.Collections.Generic;
using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Mma.Queries.GetAllClasses
{
    public class GetAllClassesQuery : IRequest<IEnumerable<ClassDto>>
    {
    }
}