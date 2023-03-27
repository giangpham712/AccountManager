using AccountManager.Application;
using AccountManager.Domain.Repositories;
using AccountManager.Persistence.MongoDb;
using AccountManager.Persistence.MongoDb.Repositories;
using Autofac;
using Microsoft.Extensions.Configuration;

namespace AccountManager.WebApi.Ioc
{
    public class MongoModule : Module
    {
        private readonly IConfiguration _configuration;

        public MongoModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var connectionString = _configuration.GetConnectionString("AccountManagerMongoDb");
            var databaseName = "AMv2";

            builder.Register(c =>
            {
                var context = new AccountManagerMongoDbContext(new MongoContextSettings
                {
                    ConnectionString = connectionString,
                    Database = databaseName
                });
                return context;
            }).As<AccountManagerMongoDbContext>().InstancePerLifetimeScope();

            builder.RegisterType<AuditLogRepository>().As<IAuditLogRepository>().InstancePerLifetimeScope();
        }
    }
}