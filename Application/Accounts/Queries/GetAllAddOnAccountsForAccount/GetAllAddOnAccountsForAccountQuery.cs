using System.Collections.Generic;
using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Accounts.Queries.GetAllAddOnAccountsForAccount
{
    public class GetAllAddOnAccountsForAccountQuery : IRequest<IEnumerable<AccountDto>>
    {
        public long Id { get; set; }
    }
}