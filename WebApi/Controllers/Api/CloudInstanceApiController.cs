using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountManager.Application;
using AccountManager.Application.Machines.Queries.GetAllCloudInstancesForMachine;
using AccountManager.Application.Machines.Queries.GetAllOperationsForMachine;
using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.WebApi.Controllers.Api
{
    [Route(ApiVersion + "/cloud-instance")]
    [ApiController]
    public class CloudInstanceApiController : AuthorizedApiController
    {
        public CloudInstanceApiController(IMediator mediator) : base(mediator)
        {
        }

        #region Queries

        [HttpGet]
        [Route("")]
        public async Task<PagedResult<CloudInstanceDto>> GetAll(
            [FromQuery] int startIndex = 0,
            [FromQuery] int limit = 20)
        {
            return await Mediator.Send(new GetAllCloudInstancesForMachineQuery() { StartIndex = startIndex, Limit = limit });
        }

        #endregion
    }
}
