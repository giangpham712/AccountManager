using System.Collections.Generic;
using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Git.Queries.GetAllBranches
{
    public class GetAllBranchesQuery : IRequest<List<BranchDto>>
    {
    }
}