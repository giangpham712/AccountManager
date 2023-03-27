using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AccountManager.Application.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountManager.WebApi.Controllers.Api
{
    [Route(ApiVersion + "/auth")]
    [AllowAnonymous]
    public class AuthApiController : ControllerBase
    {
        protected const string ApiVersion = "v1";

        public AuthApiController(IMediator mediator)
        {
            Mediator = mediator;
        }

        protected IMediator Mediator { get; }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var loginResult = await Mediator.Send(command);
            if (loginResult.Errors != null && loginResult.Errors.Any())
            {
                return Unauthorized(loginResult.Errors);
            }
            
            return Ok(new
            {
                loginResult.AccessToken,
                loginResult.RefreshToken
            });
        }

        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            return Ok();
        }
    }
}