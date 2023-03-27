using System.Collections.Generic;
using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Git.Queries.GetAllRepos
{
    public class GetAllReposQuery : IRequest<List<RepoDto>>
    {
    }
}