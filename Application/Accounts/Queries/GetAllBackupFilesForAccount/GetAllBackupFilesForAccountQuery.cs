using System.Collections.Generic;
using AccountManager.Application.Services;
using MediatR;

namespace AccountManager.Application.Accounts.Queries.GetAllBackupFilesForAccount
{
    public class GetAllBackupFilesForAccountQuery : IRequest<IEnumerable<BackupFile>>
    {
        public long Id { get; set; }
    }
}