using AccountManager.Application.Mappings;
using Autofac;
using Microsoft.Extensions.Configuration;

namespace AccountManager.WebApi.Ioc
{
    public class AutoMapperModule : Module
    {
        private readonly IConfiguration _configuration;

        public AutoMapperModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<OperationTypeValueResolver>().SingleInstance();
            builder.RegisterType<StringVersionInfoConverter>().SingleInstance();
            builder.RegisterType<AMTaskDescriptionResolver>().SingleInstance();
        }
    }
}