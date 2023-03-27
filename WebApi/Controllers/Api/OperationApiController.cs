using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountManager.Application;
using AccountManager.Application.Machines.Queries.GetAllOperationsForMachine;
using AccountManager.Application.Machines.Queries.GetDeployerLogForOperation;
using AccountManager.Application.Machines.Queries.GetOutputForOperation;
using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.WebApi.Controllers.Api
{
    [Route(ApiVersion + "/operations")]
    public class OperationApiController : AuthorizedApiController
    {
        public OperationApiController(IMediator mediator) : base(mediator)
        {
        }

        #region Queries

        [HttpGet]
        [Route("")]
        public async Task<PagedResult<OperationDto>> GetAll(
            [FromQuery] int startIndex = 0,
            [FromQuery] int limit = 20)
        {
            return await Mediator.Send(new GetAllOperationsForMachineQuery() { StartIndex = startIndex, Limit = limit });
        }

        [HttpGet]
        [Route("{id:long}/output")]
        public async Task<string> GetOutput([FromRoute] long id)
        {
            return await Mediator.Send(new GetOutputForOperationQuery() { Id = id });
        }

        [HttpGet]
        [Route("{id:long}/deployer-log")]
        public async Task<string> GetDeployerLog([FromRoute] long id)
        {
            return await Mediator.Send(new GetDeployerLogForOperationQuery() { Id = id });
        }

        #endregion
    }
}
