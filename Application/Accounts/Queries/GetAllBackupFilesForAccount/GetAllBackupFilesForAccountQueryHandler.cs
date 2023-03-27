using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Application.Services;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Accounts.Queries.GetAllBackupFilesForAccount
{
    public class
        GetAllBackupFilesForAccountQueryHandler : IRequestHandler<GetAllBackupFilesForAccountQuery,
            IEnumerable<BackupFile>>
    {
        private readonly IBackupFileService _backupFileService;
        private readonly ICloudStateDbContext _context;

        public GetAllBackupFilesForAccountQueryHandler(ICloudStateDbContext context,
            IBackupFileService backupFileService)
        {
            _context = context;
            _backupFileService = backupFileService;
        }

        public async Task<IEnumerable<BackupFile>> Handle(GetAllBackupFilesForAccountQuery request,
            CancellationToken cancellationToken)
        {
            var account = await _context.Set<Account>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);
            if (account == null) throw new EntityNotFoundException(nameof(Account), request.Id);

            var backupFiles = await _backupFileService.ListBackupFiles(account.UrlFriendlyName);
            return backupFiles;
        }
    }
}