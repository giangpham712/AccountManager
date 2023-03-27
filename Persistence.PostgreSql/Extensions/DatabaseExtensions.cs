using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace AccountManager.Persistence.PostgreSql.Extensions
{
    public static class DatabaseExtensions
    {
        public static IDbContextTransaction GetTransaction(this DatabaseFacade database)
        {
            return database.CurrentTransaction ?? database.BeginTransaction();
        }
    }
}