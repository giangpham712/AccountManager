using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace AccountManager.Application
{
    public interface ICloudStateDbContext
    {
        DatabaseFacade Database { get; }
        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        Task<int> SaveChangesAsync(CancellationToken token, out IEnumerable<EntityChangeLog> changes);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        void SetCommandTimeout(TimeSpan timeout);
        IDbContextTransaction GetTransaction();
    }
}