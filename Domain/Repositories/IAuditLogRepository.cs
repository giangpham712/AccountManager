using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Domain.Entities.Audit;

namespace AccountManager.Domain.Repositories
{
    public interface IAuditLogRepository
    {
        Task AddAsync(AuditLog auditLog);

        Task<IEnumerable<AuditLog>> FindPagedAsync(
            long? accountId = null,
            string action = null,
            int startIndex = 0,
            int limit = 10,
            SortDirection sortDirection = SortDirection.Ascending,
            CancellationToken cancellationToken = default);
    }
}