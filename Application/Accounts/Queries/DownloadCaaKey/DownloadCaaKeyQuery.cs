using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Accounts.Queries.DownloadCaaKey
{
    public class DownloadCaaKeyQuery : IRequest<DownloadableDto>
    {
        public long AccountId { get; set; }
    }
}