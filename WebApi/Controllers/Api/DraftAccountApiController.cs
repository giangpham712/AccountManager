using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountManager.Application.Accounts.Commands.CreateAccountFromDraft;
using AccountManager.Application.Accounts.Commands.CreateDraftAccount;
using AccountManager.Application.Accounts.Queries.GetDraftAccount;
using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.WebApi.Controllers.Api
{
    [Route(ApiVersion + "/draft-accounts")]
    [ApiController]
    public class DraftAccountApiController : AuthorizedApiController
    {
        public DraftAccountApiController(IMediator mediator) : base(mediator)
        {
            
        }

        #region Queries

        [HttpGet]
        [Route("{id}")]
        public async Task<DraftAccountDto> Get([FromRoute] Guid id)
        {
            return await Mediator.Send(new GetDraftAccountQuery { Id = id });
        }

        #endregion

        #region Commands

        [HttpPost]
        [Route("")]
        public async Task<Guid> CreateDraft([FromBody] CreateDraftAccountCommand command)
        {
            return await Mediator.Send(command);
        }

        #endregion
    }
}
