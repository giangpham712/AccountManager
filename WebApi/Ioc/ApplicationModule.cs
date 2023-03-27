using System.Text;
using AccountManager.Application;
using AccountManager.Application.Auth;
using AccountManager.Application.Caching;
using AccountManager.Application.Configs;
using AccountManager.Application.Identity;
using AccountManager.Application.Key;
using AccountManager.Application.License;
using AccountManager.Application.Logging;
using AccountManager.Application.Services;
using AccountManager.Application.Tasks;
using Autofac;
using Infrastructure.Auth;
using Infrastructure.Caching;
using Infrastructure.Ldap;
using Infrastructure.Logging;
using Infrastructure.S3;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AccountManager.WebApi.Ioc
{

    public class ApplicationModule : Module
    {
        private readonly IConfiguration _configuration;

        public ApplicationModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var ldapConfiguration = _configuration.GetSection("Ldap").Get<LdapConfiguration>();

            builder.RegisterType<LdapUserStore>()
                .As<IUserStore<LdapUser>>()
                .WithParameter(new TypedParameter(typeof(LdapConfiguration), ldapConfiguration))
                .InstancePerLifetimeScope();

            builder.RegisterType<LdapUserManager>()
                .As<UserManager<LdapUser>>()
                .WithParameter(new TypedParameter(typeof(LdapConfiguration), ldapConfiguration))
                .InstancePerLifetimeScope();
            

            builder.RegisterType<KeysManager>()
                .As<IKeysManager>()
                .SingleInstance();

            builder.RegisterType<S3LibraryFileService>().As<ILibraryFileService>()
                .WithParameter(new TypedParameter(typeof(S3Configuration), new S3Configuration
                {
                    AccessKey = _configuration["Aws:AccessKey"],
                    SecretKey = _configuration["Aws:SecretKey"],
                    Region = _configuration["Aws:S3:LibraryFilesRegion"],
                    Bucket = _configuration["Aws:S3:LibraryFilesBucket"]
                }))
                .InstancePerLifetimeScope();

            builder.RegisterType<S3BackupFileService>().As<IBackupFileService>()
                .WithParameter(new TypedParameter(typeof(S3Configuration), new S3Configuration
                {
                    AccessKey = _configuration["Aws:AccessKey"],
                    SecretKey = _configuration["Aws:SecretKey"],
                    Region = _configuration["Aws:S3:BackupFilesRegion"],
                    Bucket = _configuration["Aws:S3:BackupFilesBucket"]
                }))
                .InstancePerLifetimeScope();

            builder.RegisterType<S3SampleDataFileService>().As<ISampleDataFileService>()
                .WithParameter(new TypedParameter(typeof(S3Configuration), new S3Configuration
                {
                    AccessKey = _configuration["Aws:AccessKey"],
                    SecretKey = _configuration["Aws:SecretKey"],
                    Region = _configuration["Aws:S3:SampleDataFilesRegion"],
                    Bucket = _configuration["Aws:S3:SampleDataFilesBucket"]
                }))
                .InstancePerLifetimeScope();

            builder.RegisterType<S3BuildFileService>().As<IBuildFileService>()
                .WithParameter(new TypedParameter(typeof(S3Configuration), new S3Configuration
                {
                    AccessKey = _configuration["Aws:AccessKey"],
                    SecretKey = _configuration["Aws:SecretKey"],
                    Region = _configuration["Aws:S3:BuildFilesRegion"],
                    Bucket = _configuration["Aws:S3:BuildFilesBucket"]
                }))
                .InstancePerLifetimeScope();

            builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().SingleInstance();

            builder.RegisterType<JwtFactory>().As<IJwtFactory>().SingleInstance();
            builder.RegisterType<JwtTokenHandler>().As<IJwtTokenHandler>().SingleInstance();
            builder.RegisterType<TokenFactory>().As<ITokenFactory>().SingleInstance();
            builder.RegisterType<JwtTokenValidator>().As<IJwtTokenValidator>().SingleInstance();

            var signingKey = _configuration["Jwt:SigningKey"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            builder.RegisterInstance(new JwtIssuerOptions
            {
                Issuer = issuer,
                Audience = audience,
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(signingKey)),
                        SecurityAlgorithms.HmacSha256)
            }).As<JwtIssuerOptions>();

            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();

            builder.RegisterType<Logger>().As<ILogger>();

            builder.RegisterType<SoftwareVersionResolver>()
                .As<ISoftwareVersionResolver>()
                .InstancePerLifetimeScope();

            builder.RegisterType<TaskManager>().As<ITaskManager>().SingleInstance();

            builder.RegisterType<LicenseGenerator>().As<ILicenseGenerator>().InstancePerLifetimeScope();

            builder.RegisterType<DraftAccountService>().As<IDraftAccountService>().SingleInstance();

            builder.RegisterType<ConfigGenerator>().As<IConfigGenerator>().InstancePerLifetimeScope();
        }
    }
}