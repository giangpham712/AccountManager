using System.Collections.Generic;
using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Templates.Queries.GetAllInstanceSettingsTemplates
{
    public class GetAllInstanceSettingsTemplatesQuery : IRequest<IEnumerable<MachineConfigDto>>
    {
    }
}