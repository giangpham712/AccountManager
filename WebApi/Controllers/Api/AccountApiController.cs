using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AccountManager.Application.Accounts.Commands.BatchUpdateAccounts;
using AccountManager.Application.Accounts.Commands.CreateAccount;
using AccountManager.Application.Accounts.Commands.CreateAccountFromDraft;
using AccountManager.Application.Accounts.Commands.CreateDraftAccount;
using AccountManager.Application.Accounts.Commands.CreateMachine;
using AccountManager.Application.Accounts.Commands.DeleteAccount;
using AccountManager.Application.Accounts.Commands.SendCreationMail;
using AccountManager.Application.Accounts.Commands.UpdateAccount;
using AccountManager.Application.Accounts.Commands.UpdateBackupSettings;
using AccountManager.Application.Accounts.Commands.UpdateIdleSchedule;
using AccountManager.Application.Accounts.Commands.UpdateInstanceSettings;
using AccountManager.Application.Accounts.Commands.UpdateLicenseSettings;
using AccountManager.Application.Accounts.Commands.UpdateMachine;
using AccountManager.Application.Accounts.Queries.DownloadAllKeys;
using AccountManager.Application.Accounts.Queries.DownloadCaaKey;
using AccountManager.Application.Accounts.Queries.DownloadLicenseFile;
using AccountManager.Application.Accounts.Queries.DownloadLicenseKeys;
using AccountManager.Application.Accounts.Queries.GetAccount;
using AccountManager.Application.Accounts.Queries.GetAllAccounts;
using AccountManager.Application.Accounts.Queries.GetAllAddOnAccountsForAccount;
using AccountManager.Application.Accounts.Queries.GetAllBackupFilesForAccount;
using AccountManager.Application.Accounts.Queries.GetAllMachinesForAccount;
using AccountManager.Application.Accounts.Queries.GetAllSitesForAccount;
using AccountManager.Application.Accounts.Queries.GetCreatableMachinesForAccount;
using AccountManager.Application.Accounts.Queries.GetInfoForAllAccounts;
using AccountManager.Application.Accounts.Queries.GetInstanceSettingsForAccount;
using AccountManager.Application.Accounts.Queries.GetLauncherLoginsForAccount;
using AccountManager.Application.Models.Dto;
using AccountManager.Application.Services;
using AccountManager.Application.Softwares.Dtos;
using AccountManager.Application.Softwares.Queries.GetSoftwareStatus;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountManager.WebApi.Controllers.Api
{
    [Route(ApiVersion + "/accounts")]
    public class AccountApiController : AuthorizedApiController
    {
        public AccountApiController(IMediator mediator) : base(mediator)
        {
        }

        #region Queries

        [HttpGet]
        [Route("")]
        public async Task<List<AccountListingDto>> GetAll()
        {
            return await Mediator.Send(new GetAllAccountsQuery());
        }

        [HttpGet]
        [Route("info")]
        public async Task<IEnumerable<AccountStatsDto>> GetInfoForAll()
        {
            return await Mediator.Send(new GetStatsForAllAccountsQuery());
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<AccountDto> Get(long id)
        {
            return await Mediator.Send(new GetAccountQuery { Id = id });
        }

        [HttpGet]
        [Route("{id}/instance-settings")]
        public async Task<MachineConfigDto> GetInstanceSettingsForAccount(long id)
        {
            return await Mediator.Send(new GetInstanceSettingsForAccountQuery { Id = id });
        }

        [HttpGet]
        [Route("{id}/sites")]
        public async Task<IEnumerable<SiteDto>> GetAllSitesForAccount(long id)
        {
            return await Mediator.Send(new GetAllSitesForAccountQuery { Id = id });
        }

        [HttpGet]
        [Route("{id}/machines")]
        public async Task<IEnumerable<MachineDto>> GetAllMachinesForAccount(long id)
        {
            return await Mediator.Send(new GetAllMachinesForAccountQuery { Id = id });
        }

        [HttpGet]
        [Route("{id}/add-on-accounts")]
        public async Task<IEnumerable<AccountDto>> GetAllAddOnAccountsForAccount(long id)
        {
            return await Mediator.Send(new GetAllAddOnAccountsForAccountQuery { Id = id });
        }

        [HttpGet]
        [Route("{id}/creatable-machines")]
        public async Task<IEnumerable<CreatableMachineDto>> GetCreatableMachinesForAccount(long id)
        {
            return await Mediator.Send(new GetCreatableMachinesForAccountQuery { Id = id });
        }

        [HttpGet]
        [Route("{id}/backups")]
        public async Task<IEnumerable<BackupFile>> GetAllBackupFilesForAccount(long id)
        {
            return await Mediator.Send(new GetAllBackupFilesForAccountQuery { Id = id });
        }

        [HttpGet]
        [Route("{id}/files/license")]
        public async Task<IActionResult> DownloadLicenseFile(long id)
        {
            var downloadable = await Mediator.Send(new DownloadLicenseFileQuery { AccountId = id });
            return CreateFileResponse(downloadable.Content, downloadable.FileName);
        }

        [HttpGet]
        [Route("{id}/launcher-logins")]
        public async Task<IActionResult> GetLauncherLogins(long id)
        {
            return Ok(await Mediator.Send(new GetLauncherLoginsForAccountQuery { AccountId = id }));
        }

        [HttpGet]
        [Route("{id}/files/license-keys")]
        public async Task<IActionResult> DownloadLicenseKeys(long id)
        {
            var downloadable = await Mediator.Send(new DownloadLicenseKeysQuery { AccountId = id });
            return CreateFileResponse(downloadable.Content, downloadable.FileName);
        }

        [HttpGet]
        [Route("{id}/files/keys")]
        public async Task<IActionResult> DownloadAllKeys(long id)
        {
            var downloadable = await Mediator.Send(new DownloadAllKeysQuery { AccountId = id });
            return CreateFileResponse(downloadable.Content, downloadable.FileName);
        }

        [HttpGet]
        [Route("{id}/files/caa-key")]
        public async Task<IActionResult> DownloadCaaKey(long id)
        {
            var downloadable = await Mediator.Send(new DownloadCaaKeyQuery { AccountId = id });
            return CreateFileResponse(downloadable.Content, downloadable.FileName);
        }

        [HttpGet]
        [Route("{id}/software-status")]
        public async Task<IEnumerable<SoftwareStatusDto>> GetSoftwareStatusForAccount(long id)
        {
            return await Mediator.Send(new GetSoftwareStatusQuery { AccountId = id });
        }

        private FileStreamResult CreateFileResponse(byte[] content, string fileName)
        {
            var stream = new MemoryStream(content);
            return File(stream, "application/octet-stream", fileName);
        }

        #endregion

        #region Commands

        [HttpPost]
        [Route("")]
        public async Task<long> Create([FromBody] CreateAccountCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpPost]
        [Route("create-from-draft")]
        public async Task<long> CreateFromDraft([FromBody] CreateAccountFromDraftCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<Unit> Update([FromRoute] long id, [FromBody] UpdateAccountCommand command)
        {
            command.Id = id;
            return await Mediator.Send(command);
        }

        [HttpPost]
        [Route("{id}/send-creation-mail")]
        public async Task<Unit> SendCreationMail(long id)
        {
            var command = new SendCreationMailCommand { AccountId = id };
            return await Mediator.Send(command);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<Unit> Delete(long id)
        {
            return await Mediator.Send(new DeleteAccountCommand { Id = id });
        }

        [HttpPatch]
        [Route("{id}/machines/{machineId}")]
        public async Task<Unit> Update([FromRoute] long id, [FromRoute] long machineId,
            [FromBody] UpdateMachineCommand command)
        {
            command.AccountId = id;
            command.MachineId = machineId;

            return await Mediator.Send(command);
        }

        [HttpPut]
        [Route("{id}/license-settings")]
        public async Task<Unit> UpdateLicenseSettings([FromRoute] long id,
            [FromBody] UpdateLicenseSettingsCommand settingsCommand)
        {
            settingsCommand.AccountId = id;
            return await Mediator.Send(settingsCommand);
        }

        [HttpPut]
        [Route("{id}/instance-settings")]
        public async Task<Unit> UpdateInstanceSettings([FromRoute] long id,
            [FromBody] UpdateInstanceSettingsCommand command)
        {
            command.AccountId = id;
            return await Mediator.Send(command);
        }


        [HttpPut]
        [Route("{id}/backup-settings")]
        public async Task<Unit> UpdateBackupSettings([FromRoute] long id,
            [FromBody] UpdateBackupSettingsCommand command)
        {
            command.AccountId = id;
            return await Mediator.Send(command);
        }

        [HttpPut]
        [Route("{id}/idle-schedule")]
        public async Task<Unit> UpdateIdleSchedule([FromRoute] long id,
            [FromBody] UpdateIdleScheduleCommand command)
        {
            command.AccountId = id;
            return await Mediator.Send(command);
        }

        [HttpPut]
        [Route("batch")]
        public async Task<Unit> BatchUpdate(BatchUpdateAccountsCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpPost]
        [Route("{id}/machines")]
        public async Task<Unit> CreateMachineForAccount([FromRoute] long id,
            [FromBody] CreateMachineCommand command)
        {
            command.AccountId = id;
            return await Mediator.Send(command);
        }

        #endregion
    }
}