using AccountManager.Application;
using AccountManager.Persistence.PostgreSql;
using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AccountManager.WebApi.Ioc
{
    public class EfModule : Module
    {
        private readonly IConfiguration _configuration;

        public EfModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        protected override void Load(ContainerBuilder builder)
        {
            var connectionString = _configuration.GetConnectionString("CloudStateDb");
            var optionsBuilder = new DbContextOptionsBuilder<CloudStatePostgresDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            builder.Register(x => new CloudStatePostgresDbContext(optionsBuilder.Options))
                .As<ICloudStateDbContext>()
                .InstancePerLifetimeScope();
        }
    }
}