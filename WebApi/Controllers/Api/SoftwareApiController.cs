using System.Collections.Generic;
using System.Threading.Tasks;
using AccountManager.Application;
using AccountManager.Application.BinaryBuild.Queries.GetAllBuildsForSoftware;
using AccountManager.Application.Git.Commands.SetCommitBreaking;
using AccountManager.Application.Git.Queries.GetAllBranches;
using AccountManager.Application.Git.Queries.GetAllRepos;
using AccountManager.Application.Git.Queries.GetCommitsForSoftware;
using AccountManager.Application.Models.Dto;
using AccountManager.Application.Services;
using AccountManager.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountManager.WebApi.Controllers.Api
{
    [Route(ApiVersion + "/software")]
    public class SoftwareApiController : AuthorizedApiController
    {
        public SoftwareApiController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        [Route("repos")]
        public async Task<IEnumerable<RepoDto>> GetAllRepos()
        {
            return await Mediator.Send(new GetAllReposQuery());
        }

        [HttpGet]
        [Route("{software}/commits")]
        public async Task<PagedResult<CommitDto>> GetCommitsForRepo(Software software, [FromQuery] int? branch = null,
            [FromQuery] bool tagsOnly = false, [FromQuery] bool buildsOnly = false, [FromQuery] string search = null,
            [FromQuery] int startIndex = 0, [FromQuery] int limit = 20)
        {
            return await Mediator.Send(new GetCommitsForSoftwareQuery
            {
                Software = software,
                BranchId = branch,
                TagsOnly = tagsOnly,
                BuildsOnly = buildsOnly,
                Search = search,
                StartIndex = startIndex,
                Limit = limit
            });
        }

        [HttpGet]
        [Route("branches")]
        public async Task<IEnumerable<BranchDto>> GetAllBranches()
        {
            return await Mediator.Send(new GetAllBranchesQuery());
        }

        [HttpGet]
        [Route("{software}/builds")]
        public async Task<IEnumerable<BuildFile>> GetBuildsForSoftware([FromRoute] Software software,
            [FromRoute] BuildFileType type)
        {
            return await Mediator.Send(new GetAllBuildsForSoftwareQuery
            {
                Software = software,
                Type = type
            });
        }

        #region Commands

        [HttpPut]
        [Route("{software}/commits/{hash}/breaking")]
        public async Task<Unit> SetCommitBreaking([FromRoute] Software software, [FromRoute] string hash,
            [FromBody] SetCommitBreakingCommand command)
        {
            command.Software = software;
            command.Hash = hash;
            return await Mediator.Send(command);
        }

        #endregion
    }
}