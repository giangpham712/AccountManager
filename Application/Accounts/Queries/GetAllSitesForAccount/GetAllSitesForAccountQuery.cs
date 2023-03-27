using System.Collections.Generic;
using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Accounts.Queries.GetAllSitesForAccount
{
    public class GetAllSitesForAccountQuery : IRequest<IEnumerable<SiteDto>>
    {
        public long Id { get; set; }
    }
}