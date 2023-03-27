using System.Collections.Generic;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Application.Templates.Commands.CreateOrUpdateBackupSettingsTemplate;
using AccountManager.Application.Templates.Commands.CreateOrUpdateGeneralTemplate;
using AccountManager.Application.Templates.Commands.CreateOrUpdateInstanceSettingsTemplate;
using AccountManager.Application.Templates.Commands.CreateOrUpdateLicenseTemplate;
using AccountManager.Application.Templates.Commands.DeleteTemplate;
using AccountManager.Application.Templates.Queries.GetAllBackupSettingsTemplates;
using AccountManager.Application.Templates.Queries.GetAllGeneralTemplates;
using AccountManager.Application.Templates.Queries.GetAllInstanceSettingsTemplates;
using AccountManager.Application.Templates.Queries.GetAllLicenseTemplates;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountManager.WebApi.Controllers.Api
{
    [Route(ApiVersion + "/template")]
    public class TemplateApiController : AuthorizedApiController
    {
        public TemplateApiController(IMediator mediator) : base(mediator)
        {
        }

        #region Queries

        [HttpGet]
        [Route("license-settings")]
        public async Task<IEnumerable<LicenseConfigDto>> GetAllLicenseTemplates()
        {
            return await Mediator.Send(new GetAllLicenseTemplatesQuery());
        }

        [HttpGet]
        [Route("instance-settings")]
        public async Task<IEnumerable<MachineConfigDto>> GetAllInstanceSettingsTemplates()
        {
            return await Mediator.Send(new GetAllInstanceSettingsTemplatesQuery());
        }

        [HttpGet]
        [Route("backup-settings")]
        public async Task<IEnumerable<BackupConfigDto>> GetAllBackupSettingsTemplates()
        {
            return await Mediator.Send(new GetAllBackupSettingsTemplatesQuery());
        }

        [HttpGet]
        [Route("general")]
        public async Task<IEnumerable<AccountDto>> GetAllGeneralTemplates()
        {
            return await Mediator.Send(new GetAllGeneralTemplatesQuery());
        }

        #endregion

        #region Commands

        [HttpPost]
        [Route("license-settings")]
        public async Task<long> CreateLicenseTemplate(
            [FromBody] CreateOrUpdateLicenseTemplateCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpPost]
        [Route("instance-settings")]
        public async Task<long> CreateInstanceSettingsTemplate(
            [FromBody] CreateOrUpdateInstanceSettingsTemplateCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpPost]
        [Route("backup-settings")]
        public async Task<long> CreateBackupSettingsTemplate(
            [FromBody] CreateOrUpdateBackupSettingsTemplateCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpPost]
        [Route("general")]
        public async Task<long> CreateGeneralTemplate(
            [FromBody] CreateOrUpdateGeneralTemplateCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpPut]
        [Route("license-settings/{id}")]
        public async Task<long> UpdateLicenseTemplate(long id,
            [FromBody] CreateOrUpdateLicenseTemplateCommand command)
        {
            command.Id = id;
            return await Mediator.Send(command);
        }

        [HttpPut]
        [Route("instance-settings/{id}")]
        public async Task<long> UpdateInstanceSettingsTemplate(long id,
            [FromBody] CreateOrUpdateInstanceSettingsTemplateCommand command)
        {
            command.Id = id;
            return await Mediator.Send(command);
        }

        [HttpPut]
        [Route("backup-settings/{id}")]
        public async Task<long> UpdateBackupSettingsTemplate(long id,
            [FromBody] CreateOrUpdateBackupSettingsTemplateCommand command)
        {
            command.Id = id;
            return await Mediator.Send(command);
        }

        [HttpPut]
        [Route("general/{id}")]
        public async Task<long> UpdateGeneralTemplate(long id,
            [FromBody] CreateOrUpdateGeneralTemplateCommand command)
        {
            command.Id = id;
            return await Mediator.Send(command);
        }

        [HttpDelete]
        [Route("{type}/{id}")]
        public async Task<Unit> DeleteTemplate([FromRoute] string type, [FromRoute] long id)
        {
            return await Mediator.Send(new DeleteTemplateCommand
            {
                Id = id,
                Type = type
            });
        }

        #endregion
    }
}