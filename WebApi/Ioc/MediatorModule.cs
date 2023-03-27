using System.Reflection;
using AccountManager.Application.Accounts.Commands.CreateAccount;
using AccountManager.Application.Accounts.Queries.GetAllAccounts;
using AccountManager.Application.Audit.Events;
using AccountManager.Application.Infrastructure;
using Autofac;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Module = Autofac.Module;

namespace AccountManager.WebApi.Ioc
{
    public class MediatorModule : Module
    {
        private readonly IConfiguration _configuration;

        public MediatorModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Mediator>()
                .As<IMediator>()
                .InstancePerLifetimeScope();

            builder.Register<ServiceFactory>(context =>
            {
                var c = context.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });

            builder.RegisterAssemblyTypes(typeof(GetAllAccountsQuery).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IRequestHandler<,>));

            builder.RegisterAssemblyTypes(typeof(UserActionNotification).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(INotificationHandler<>));

            builder
                .RegisterAssemblyTypes(typeof(CreateAccountCommandValidator).GetTypeInfo().Assembly)
                .Where(t => t.IsClosedTypeOf(typeof(IValidator<>)))
                .AsImplementedInterfaces();

            builder.RegisterGeneric(typeof(RequestBehavior<,>)).As(typeof(IPipelineBehavior<,>));
        }
    }
}