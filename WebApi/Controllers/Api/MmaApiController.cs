using System.Collections.Generic;
using System.Threading.Tasks;
using AccountManager.Application.Mma.Commands.UpdateMmaClass;
using AccountManager.Application.Mma.Queries.GetAllClasses;
using AccountManager.Application.Mma.Queries.GetAllInstances;
using AccountManager.Application.Mma.Queries.GetAllOperationTypes;
using AccountManager.Application.Mma.Queries.GetMmaStats;
using AccountManager.Application.Models.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountManager.WebApi.Controllers.Api
{
    [Route(ApiVersion + "/mma")]
    public class MmaApiController : AuthorizedApiController
    {
        public MmaApiController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        [Route("classes")]
        public async Task<IEnumerable<ClassDto>> GetAllClasses()
        {
            return await Mediator.Send(new GetAllClassesQuery());
        }

        [HttpGet]
        [Route("instances")]
        public async Task<IEnumerable<MmaInstanceDto>> GetAllInstances()
        {
            return await Mediator.Send(new GetAllMmaInstancesQuery());
        }

        [HttpGet]
        [Route("operation-types")]
        public async Task<IEnumerable<OperationTypeDto>> GetAllOperationTypes()
        {
            return await Mediator.Send(new GetAllOperationTypesQuery());
        }

        [HttpPut]
        [Route("classes/{id}")]
        public async Task<ClassDto> UpdateClass([FromRoute] long id, [FromBody] UpdateMmaClassCommand command)
        {
            command.Id = id;
            return await Mediator.Send(command);
        }

        [HttpGet]
        [Route("stats")]
        public async Task<MmaStatsDto> GetMmaStats()
        {
            return await Mediator.Send(new GetMmaStatsQuery());
        }
    }
}