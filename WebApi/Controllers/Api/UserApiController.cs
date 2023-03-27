using System.Threading.Tasks;
using AccountManager.Application.Auth.Queries;
using AccountManager.Application.Models.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountManager.WebApi.Controllers.Api
{
    [Route(ApiVersion + "/user")]
    public class UserApiController : AuthorizedApiController
    {
        public UserApiController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        [Route("me")]
        public async Task<UserDto> Me()
        {
            return await Mediator.Send(new GetCurrentUserQuery());
        }
    }
}