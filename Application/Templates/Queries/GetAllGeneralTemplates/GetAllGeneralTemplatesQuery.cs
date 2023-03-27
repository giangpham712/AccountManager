using System.Collections.Generic;
using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Templates.Queries.GetAllGeneralTemplates
{
    public class GetAllGeneralTemplatesQuery : IRequest<IEnumerable<AccountDto>>
    {
    }
}