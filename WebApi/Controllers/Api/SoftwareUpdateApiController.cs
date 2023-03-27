using System.Threading.Tasks;
using AccountManager.Application.SoftwareUpdate.UpdateSoftwareForAccounts;
using AccountManager.Application.SoftwareUpdate.UpdateSoftwareForMachines;
using AccountManager.Application.SoftwareUpdate.ValidateSoftwareUpdate;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountManager.WebApi.Controllers.Api
{
    [Route(ApiVersion + "/software-update")]
    public class SoftwareUpdateApiController : AuthorizedApiController
    {
        public SoftwareUpdateApiController(IMediator mediator) : base(mediator)
        {
        }

        #region Commands

        [HttpPost]
        [Route("update-machines")]
        public async Task<Unit> UpdateMachines([FromBody] UpdateSoftwareForMachinesCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpPost]
        [Route("update-accounts")]
        public async Task<Unit> UpdateAccounts([FromBody] UpdateSoftwareForAccountsCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpPost]
        [Route("validate")]
        public async Task<ValidateSoftwareUpdateResult> Validate([FromBody] ValidateSoftwareUpdateCommand command)
        {
            return await Mediator.Send(command);
        }

        #endregion
    }
}