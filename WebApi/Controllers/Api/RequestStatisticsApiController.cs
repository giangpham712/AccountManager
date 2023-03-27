using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountManager.Application;
using AccountManager.Application.Machines.Queries.GetRequestStatisticsForMachine;
using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.WebApi.Controllers.Api
{
    [Route(ApiVersion + "/request-statistics")]
    public class RequestStatisticsApiController : AuthorizedApiController
    {
        public RequestStatisticsApiController(IMediator mediator) : base(mediator)
        {

        }

        #region Queries

        [HttpGet]
        [Route("")]
        public async Task<PagedResult<RequestStatisticsDto>> GetAll(
            [FromQuery] int startIndex = 0,
            [FromQuery] int limit = 20)
        {
            return await Mediator.Send(new GetRequestStatisticsForMachineQuery() { StartIndex = startIndex, Limit = limit });
        }

        #endregion'
    }
}
