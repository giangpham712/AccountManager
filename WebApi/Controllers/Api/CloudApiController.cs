using System.Collections.Generic;
using System.Threading.Tasks;
using AccountManager.Application.Cloud.Queries.GetAllImages;
using AccountManager.Application.Cloud.Queries.GetAllInstanceTypes;
using AccountManager.Application.Cloud.Queries.GetAllRedisInstanceTypes;
using AccountManager.Application.Cloud.Queries.GetAllRegions;
using AccountManager.Application.Models.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountManager.WebApi.Controllers.Api
{
    [Route(ApiVersion + "/cloud")]
    public class CloudApiController : AuthorizedApiController
    {
        public CloudApiController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        [Route("regions")]
        public async Task<List<RegionDto>> GetAllRegions()
        {
            return await Mediator.Send(new GetAllRegionsQuery());
        }

        [HttpGet]
        [Route("instance-types")]
        public async Task<List<CloudInstanceTypeDto>> GetAllInstanceTypes()
        {
            return await Mediator.Send(new GetAllInstanceTypesQuery());
        }

        [HttpGet]
        [Route("redis-instance-types")]
        public async Task<List<RedisCloudInstanceTypeDto>> GetAllRedisInstanceTypes()
        {
            return await Mediator.Send(new GetAllRedisInstanceTypesQuery());
        }

        [HttpGet]
        [Route("images")]
        public async Task<List<CloudBaseImageDto>> GetAllImages()
        {
            return await Mediator.Send(new GetAllImagesQuery());
        }
    }
}