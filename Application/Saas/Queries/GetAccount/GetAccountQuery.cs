using AccountManager.Application.Models.Dto.Saas;
using MediatR;

namespace AccountManager.Application.Saas.Queries.GetAccount
{
    public class GetAccountQuery : IRequest<SaasAccountDto>
    {
        public string UrlFriendlyName { get; set; }
    }
}