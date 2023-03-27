using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Accounts.Queries.DownloadCaaKey
{
    public class DownloadCaaKeyQueryHandler : IRequestHandler<DownloadCaaKeyQuery, DownloadableDto>
    {
        private readonly ICloudStateDbContext _context;

        public DownloadCaaKeyQueryHandler(ICloudStateDbContext context)
        {
            _context = context;
        }

        public async Task<DownloadableDto> Handle(DownloadCaaKeyQuery request, CancellationToken cancellationToken)
        {
            var account = await _context.Set<Account>()
                .Include(x => x.Keys)
                .Where(x => x.Id == request.AccountId)
                .FirstOrDefaultAsync(cancellationToken);

            if (account == null)
                throw new EntityNotFoundException(nameof(Account), request.AccountId);

            using (var memoryStream = new MemoryStream())
            {
                var apiPassword = account.Keys.ApiPassword;
                var bytes = Encoding.ASCII.GetBytes($"{account.UrlFriendlyName}:{apiPassword}");
                return new DownloadableDto
                {
                    Content = bytes,
                    FileName = "caa-account.txt"
                };
            }
        }
    }
}