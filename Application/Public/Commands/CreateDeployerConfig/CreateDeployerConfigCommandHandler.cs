using System.Threading;
using System.Threading.Tasks;
using AccountManager.Domain.Entities.Public;
using AutoMapper;
using MediatR;

namespace AccountManager.Application.Public.Commands.CreateDeployerConfig
{
    public class CreateDeployerConfigCommandHandler : CommandHandlerBase<CreateDeployerConfigCommand, Unit>
    {
        private readonly IMapper _mapper;

        public CreateDeployerConfigCommandHandler(IMediator mediator, ICloudStateDbContext context, IMapper mapper) : base(mediator, context)
        {
            _mapper = mapper;
        }

        public override async Task<Unit> Handle(CreateDeployerConfigCommand command, CancellationToken cancellationToken)
        {
            var componentConfig = _mapper.Map<DeployerConfig>(command);

            await Context.Set<DeployerConfig>().AddAsync(componentConfig, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}