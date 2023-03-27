using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Domain.Entities.Public;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Public.Commands.UpdateDeployerConfig
{
    public class UpdateDeployerConfigCommandHandler : CommandHandlerBase<UpdateDeployerConfigCommand, Unit>
    {
        public UpdateDeployerConfigCommandHandler(IMediator mediator, ICloudStateDbContext context) : base(mediator, context)
        {
        }

        public override async Task<Unit> Handle(UpdateDeployerConfigCommand command, CancellationToken cancellationToken)
        {
            var deployerConfig = await Context.Set<DeployerConfig>()
                .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

            if (deployerConfig == null)
            {
                throw new EntityNotFoundException(nameof(DeployerConfig), command.Id);
            }

            deployerConfig.DefaultValue = command.DefaultValue;
            deployerConfig.Protected = command.Protected;
            deployerConfig.DataType = command.DataType;

            await Context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}