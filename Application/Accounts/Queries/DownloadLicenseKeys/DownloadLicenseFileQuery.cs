using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Accounts.Queries.DownloadLicenseKeys
{
    public class DownloadLicenseKeysQuery : IRequest<DownloadableDto>
    {
        public long AccountId { get; set; }
    }
}