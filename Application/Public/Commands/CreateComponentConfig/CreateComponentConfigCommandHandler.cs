using System;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Domain.Entities.Public;
using AutoMapper;
using MediatR;

namespace AccountManager.Application.Public.Commands.CreateComponentConfig
{
    public class CreateComponentConfigCommandHandler : CommandHandlerBase<CreateComponentConfigCommand, Unit>
    {
        private readonly IMapper _mapper;

        public CreateComponentConfigCommandHandler(IMediator mediator, ICloudStateDbContext context, IMapper mapper) : base(mediator, context)
        {
            _mapper = mapper;
        }

        public override async Task<Unit> Handle(CreateComponentConfigCommand command, CancellationToken cancellationToken)
        {
            var componentConfig = _mapper.Map<ComponentConfig>(command);

            await Context.Set<ComponentConfig>().AddAsync(componentConfig, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}