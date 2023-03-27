using System.Collections.Generic;
using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Templates.Queries.GetAllBackupSettingsTemplates
{
    public class GetAllBackupSettingsTemplatesQuery : IRequest<IEnumerable<BackupConfigDto>>
    {
    }
}