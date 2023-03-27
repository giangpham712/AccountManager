using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountManager.WebApi.Controllers.Api
{
    [Route(ApiVersion + "/admin")]
    public class AdminApiController : AuthorizedApiController
    {
        public AdminApiController(IMediator mediator) : base(mediator)
        {
        }
    }
}