using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application;
using AccountManager.Domain;
using AccountManager.Domain.Entities;
using AccountManager.Persistence.PostgreSql.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AccountManager.Persistence.PostgreSql
{
    public class CloudStatePostgresDbContext : DbContext, ICloudStateDbContext
    {
        public CloudStatePostgresDbContext(DbContextOptions<CloudStatePostgresDbContext> options) 
            : base(options)
        {
        }

        public new DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }

        public Task<int> SaveChangesAsync(CancellationToken token, out IEnumerable<EntityChangeLog> changes)
        {
            changes = ChangeTracker.Entries().Where(x => x.State == EntityState.Modified).SelectMany(x =>
            {
                var entity = x.Entity as IEntity;

                var entityChanges = new List<EntityChangeLog>();
                foreach (var property in x.OriginalValues.Properties)
                {
                    var originalValue = x.OriginalValues[property]?.ToString();
                    var currentValue = x.CurrentValues[property]?.ToString();
                    if (originalValue != currentValue)
                        entityChanges.Add(new EntityChangeLog
                        {
                            EntityType = x.Entity.GetType().Name,
                            EntityName = x.Entity.GetType().Name,
                            EntityId = entity?.Id ?? 0,
                            PropertyName = property.Name,
                            OldValue = x.OriginalValues[property],
                            NewValue = x.CurrentValues[property]
                        });
                }

                return entityChanges;
            }).ToList();

            return SaveChangesAsync(token);
        }

        public void SetCommandTimeout(TimeSpan timeout)
        {
            Database.SetCommandTimeout(timeout);
        }

        public IDbContextTransaction GetTransaction()
        {
            return Database.CurrentTransaction ?? Database.BeginTransaction();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var assembly = Assembly.GetExecutingAssembly();
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}