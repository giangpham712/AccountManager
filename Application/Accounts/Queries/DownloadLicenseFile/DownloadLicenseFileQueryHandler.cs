using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Accounts.Queries.DownloadLicenseFile
{
    public class DownloadLicenseFileQueryHandler : IRequestHandler<DownloadLicenseFileQuery, DownloadableDto>
    {
        private readonly ICloudStateDbContext _context;

        public DownloadLicenseFileQueryHandler(ICloudStateDbContext context)
        {
            _context = context;
        }

        public async Task<DownloadableDto> Handle(DownloadLicenseFileQuery request, CancellationToken cancellationToken)
        {
            var account = await _context.Set<Account>()
                .Where(x => x.Id == request.AccountId)
                .FirstOrDefaultAsync(cancellationToken);

            if (account == null)
                throw new EntityNotFoundException(nameof(Account), request.AccountId);

            var licenseFileStr = account.License;

            var bytes = Convert.FromBase64String(licenseFileStr);

            return new DownloadableDto
            {
                Content = bytes,
                FileName = $"{account.UrlFriendlyName} license.dat"
            };
        }
    }
}