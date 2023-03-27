using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Accounts.Queries.DownloadAllKeys
{
    public class DownloadAllKeysQuery : IRequest<DownloadableDto>
    {
        public long AccountId { get; set; }
    }
}