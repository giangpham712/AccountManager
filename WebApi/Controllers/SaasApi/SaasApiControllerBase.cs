using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountManager.WebApi.Controllers.SaasApi
{
    public abstract class SaasApiControllerBase : ControllerBase
    {
        protected const string ApiVersion = "v1";

        protected SaasApiControllerBase(IMediator mediator)
        {
            Mediator = mediator;
        }

        protected IMediator Mediator { get; }
    }
}