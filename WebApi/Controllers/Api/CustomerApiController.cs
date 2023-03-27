using System.Collections.Generic;
using System.Threading.Tasks;
using AccountManager.Application.Accounts.Queries.GetAllCustomers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountManager.WebApi.Controllers.Api
{
    [Route(ApiVersion + "/customers")]
    public class CustomerApiController : AuthorizedApiController
    {
        public CustomerApiController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        [Route("")]
        public async Task<IEnumerable<string>> GetAll()
        {
            return await Mediator.Send(new GetAllCustomersQuery());
        }
    }
}