using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountManager.WebApi.Controllers.Api
{
    [Authorize]
    public abstract class AuthorizedApiController : ControllerBase
    {
        protected const string ApiVersion = "v1";

        protected AuthorizedApiController(IMediator mediator)
        {
            Mediator = mediator;
        }

        protected IMediator Mediator { get; }
    }
}