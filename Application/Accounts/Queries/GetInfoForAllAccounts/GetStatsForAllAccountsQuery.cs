using System.Collections.Generic;
using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Accounts.Queries.GetInfoForAllAccounts
{
    public class GetStatsForAllAccountsQuery : IRequest<IEnumerable<AccountStatsDto>>
    {
    }
}