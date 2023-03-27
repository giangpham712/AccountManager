using System.Collections.Generic;
using System.Threading.Tasks;
using AccountManager.Application.Cloud.Queries.GetAllInstanceTypes;
using AccountManager.Application.Models.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountManager.WebApi.Controllers.SaasApi
{
    [Route(ApiVersion + "/saas/cloud")]
    public class SaasCloudApiController : SaasApiControllerBase
    {
        public SaasCloudApiController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        [Route("instance-types")]
        public async Task<IEnumerable<CloudInstanceTypeDto>> ListInstanceTypes()
        {
            var query = new GetAllInstanceTypesQuery();
            return await Mediator.Send(query);
        }
    }
}