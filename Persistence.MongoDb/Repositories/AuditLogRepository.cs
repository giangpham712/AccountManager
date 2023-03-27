using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Common.Extensions;
using AccountManager.Domain.Entities.Audit;
using AccountManager.Domain.Repositories;
using MongoDB.Driver;
using SortDirection = AccountManager.Domain.SortDirection;

namespace AccountManager.Persistence.MongoDb.Repositories
{
    public class AuditLogRepository : MongoRepositoryBase<AuditLog>, IAuditLogRepository
    {
        public AuditLogRepository(AccountManagerMongoDbContext context) : base(context)
        {
        }

        protected override string CollectionName => "AuditLogs";

        public async Task<IEnumerable<AuditLog>> FindPagedAsync(long? accountId = null, string action = null,
            int startIndex = 0, int limit = 10,
            SortDirection sortDirection = SortDirection.Ascending,
            CancellationToken cancellationToken = default)
        {
            var filter = Builders<AuditLog>.Filter.Empty;

            if (accountId.HasValue)
                filter = filter & Builders<AuditLog>.Filter.ElemMatch(x => x.Accounts, x => x.Id == accountId);

            if (!action.IsNullOrWhiteSpace()) filter = filter & Builders<AuditLog>.Filter.Eq(x => x.Action, action);


            SortDefinition<AuditLog> sort;
            if (sortDirection == SortDirection.Ascending)
                sort = Builders<AuditLog>.Sort.Ascending(x => x.Time);
            else
                sort = Builders<AuditLog>.Sort.Descending(x => x.Time);

            var options = new FindOptions<AuditLog>
            {
                Sort = sort,
                Limit = limit,
                Skip = startIndex
            };

            var cursor = await Context.AuditLogs.FindAsync(filter, options, cancellationToken);

            return await cursor.ToListAsync(cancellationToken);
        }
    }
}