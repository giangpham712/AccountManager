using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Domain.Entities.Public;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Public.Commands.DeleteComponentConfig
{
    public class DeleteComponentConfigCommand : CommandBase
    {
        public long Id { get; set; }
    }

    public class DeleteComponentConfigCommandHandler : CommandHandlerBase<DeleteComponentConfigCommand, Unit>
    {
        public DeleteComponentConfigCommandHandler(IMediator mediator, ICloudStateDbContext context) : base(mediator, context)
        {
        }

        public override async Task<Unit> Handle(DeleteComponentConfigCommand command, CancellationToken cancellationToken)
        {
            var componentConfig = await Context.Set<ComponentConfig>()
                .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

            if (componentConfig == null)
            {
                throw new EntityNotFoundException(nameof(ComponentConfig), command.Id);
            }

            Context.Set<ComponentConfig>().Remove(componentConfig);

            await Context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
