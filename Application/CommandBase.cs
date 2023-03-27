using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace AccountManager.Application
{
    public abstract class CommandBase : CommandBase<Unit>, IRequest
    {
    }

    public abstract class CommandBase<T> : IRequest<T>, ICommand
    {
        public string User { get; set; }
    }

    public interface ICommand
    {
        string User { get; set; }
    }

    public abstract class CommandHandlerBase<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        protected readonly ICloudStateDbContext Context;
        protected readonly IMediator Mediator;

        protected CommandHandlerBase(IMediator mediator, ICloudStateDbContext context)
        {
            Mediator = mediator;
            Context = context;
        }

        public abstract Task<TResponse> Handle(TRequest command, CancellationToken cancellationToken);
    }
}