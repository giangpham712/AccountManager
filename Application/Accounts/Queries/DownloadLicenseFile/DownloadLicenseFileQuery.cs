using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Accounts.Queries.DownloadLicenseFile
{
    public class DownloadLicenseFileQuery : IRequest<DownloadableDto>
    {
        public long AccountId { get; set; }
    }
}