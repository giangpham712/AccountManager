using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Accounts.Queries.GetAccount
{
    public class GetAccountQuery : IRequest<AccountDto>
    {
        public long Id { get; set; }
    }
}