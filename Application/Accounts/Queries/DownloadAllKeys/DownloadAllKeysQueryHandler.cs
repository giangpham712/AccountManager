using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Accounts.Queries.DownloadAllKeys
{
    public class DownloadAllKeysQueryHandler : IRequestHandler<DownloadAllKeysQuery, DownloadableDto>
    {
        private readonly ICloudStateDbContext _context;

        public DownloadAllKeysQueryHandler(ICloudStateDbContext context)
        {
            _context = context;
        }

        public async Task<DownloadableDto> Handle(DownloadAllKeysQuery request, CancellationToken cancellationToken)
        {
            var account = await _context.Set<Account>()
                .Include(x => x.Keys)
                .Where(x => x.Id == request.AccountId)
                .FirstOrDefaultAsync(cancellationToken);

            if (account == null)
                throw new EntityNotFoundException(nameof(Account), request.AccountId);

            using (var memoryStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    var licensePublicKeyEntry = zipArchive.CreateEntry("LicenseKey.public");
                    using (var writer = new StreamWriter(licensePublicKeyEntry.Open()))
                    {
                        writer.Write(account.Keys.LicensePublic);
                    }

                    var licensePrivateKeyEntry = zipArchive.CreateEntry("LicenseKey.private");
                    using (var writer = new StreamWriter(licensePrivateKeyEntry.Open()))
                    {
                        writer.Write(account.Keys.LicensePrivate);
                    }

                    var userPrivateKeyEntry = zipArchive.CreateEntry("UserTokenKey.private");
                    using (var writer = new StreamWriter(userPrivateKeyEntry.Open()))
                    {
                        writer.Write(account.Keys.UserTokenPrivate);
                    }

                    var userPublicKeyEntry = zipArchive.CreateEntry("UserTokenKey.public");
                    using (var writer = new StreamWriter(userPublicKeyEntry.Open()))
                    {
                        writer.Write(account.Keys.UserTokenPublic);
                    }

                    var interServerPrivateKey = zipArchive.CreateEntry("InterServerKey.private");
                    using (var writer = new StreamWriter(interServerPrivateKey.Open()))
                    {
                        writer.Write(account.Keys.InterServerPrivate);
                    }

                    var interServerPublicKey = zipArchive.CreateEntry("InterServerKey.public");
                    using (var writer = new StreamWriter(interServerPublicKey.Open()))
                    {
                        writer.Write(account.Keys.InterServerPublic);
                    }
                }

                var bytes = memoryStream.ToArray();
                return new DownloadableDto
                {
                    Content = bytes,
                    FileName = $"{account.UrlFriendlyName} keys.zip"
                };
            }
        }
    }
}