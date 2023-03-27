using System.Collections.Generic;
using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Templates.Queries.GetAllLicenseTemplates
{
    public class GetAllLicenseTemplatesQuery : IRequest<IEnumerable<LicenseConfigDto>>
    {
    }
}