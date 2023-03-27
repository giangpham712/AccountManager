using System.Collections.Generic;
using System.Threading.Tasks;
using AccountManager.Application.Accounts.Queries.GetAllMachinesForAccount;
using AccountManager.Application.Models.Dto;
using AccountManager.Application.Models.Dto.Saas;
using AccountManager.Application.Notification.Commands.SendMessage;
using AccountManager.Application.Saas.Commands.CreateSite;
using AccountManager.Application.Saas.Commands.DeleteSite;
using AccountManager.Application.Saas.Queries.GetAccount;
using AccountManager.Application.Saas.Queries.GetSiteStatus;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountManager.WebApi.Controllers.SaasApi
{
    [Route(ApiVersion + "/saas/accounts")]
    public class SaasAccountApiController : SaasApiControllerBase
    {
        public SaasAccountApiController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        [Route("{urlFriendlyName}")]
        public async Task<SaasAccountDto> GetAccount([FromRoute] string urlFriendlyName)
        {
            var query = new GetAccountQuery
            {
                UrlFriendlyName = urlFriendlyName
            };
            return await Mediator.Send(query);
        }

        [HttpGet]
        [Route("{urlFriendlyName}/machines")]
        public async Task<IEnumerable<MachineDto>> GetMachines([FromRoute] string urlFriendlyName)
        {
            var query = new GetAllMachinesForAccountQuery
            {
                UrlFriendlyName = urlFriendlyName
            };

            return await Mediator.Send(query);
        }

        [HttpPost]
        [Route("{urlFriendlyName}/sites")]
        public async Task<Unit> CreateSite([FromRoute] string urlFriendlyName,
            [FromBody] CreateSiteCommand command)
        {
            command.AccountUrlFriendlyName = urlFriendlyName;
            return await Mediator.Send(command);
        }

        [HttpGet]
        [Route("{urlFriendlyName}/sites/{siteUrlFriendlyName}/status")]
        public async Task<SiteStatusDto> GetSiteStatus([FromRoute] string urlFriendlyName,
            [FromRoute] string siteUrlFriendlyName)
        {
            var query = new GetSiteStatusQuery
            {
                AccountUrlFriendlyName = urlFriendlyName,
                UrlFriendlyName = siteUrlFriendlyName
            };

            return await Mediator.Send(query);
        }

        [HttpDelete]
        [Route("{urlFriendlyName}/sites/{siteUrlFriendlyName}")]
        public async Task<Unit> DeleteSite([FromRoute] string urlFriendlyName,
            [FromRoute] string siteUrlFriendlyName)
        {
            var command = new DeleteSiteCommand();
            command.AccountUrlFriendlyName = urlFriendlyName;
            command.UrlFriendlyName = siteUrlFriendlyName;
            return await Mediator.Send(command);
        }

        [HttpPost]
        [Route("{urlFriendlyName}/notifications")]
        public async Task<Unit> CreateNotification([FromRoute] string urlFriendlyName,
            [FromBody] SendMessageCommand command)
        {
            command.Accounts = null;
            command.AccountUrlNames = new[] { urlFriendlyName };
            return await Mediator.Send(command);
        }
    }
}