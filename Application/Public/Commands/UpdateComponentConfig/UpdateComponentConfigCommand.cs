using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Domain.Entities.Public;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Public.Commands.UpdateComponentConfig
{
    public class UpdateComponentConfigCommand : CommandBase
    {
        public long Id { get; set; }

        public string SaasDefaultValue { get; set; }

        public string OnPremDefaultValue { get; set; }

        public bool SiteSpecific { get; set; }

        public string DataType { get; set; }

        public bool Protected { get; set; }
    }

    public class UpdateComponentConfigCommandHandler : CommandHandlerBase<UpdateComponentConfigCommand, Unit>
    {
        public UpdateComponentConfigCommandHandler(IMediator mediator, ICloudStateDbContext context) : base(mediator, context)
        {
        }

        public override async Task<Unit> Handle(UpdateComponentConfigCommand command, CancellationToken cancellationToken)
        {
            var componentConfig = await Context.Set<ComponentConfig>()
                .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

            if (componentConfig == null)
            {
                throw new EntityNotFoundException(nameof(ComponentConfig), command.Id);
            }

            componentConfig.SaasDefaultValue = command.SaasDefaultValue;
            componentConfig.OnPremDefaultValue = command.OnPremDefaultValue;
            componentConfig.Protected = command.Protected;
            componentConfig.SiteSpecific = command.SiteSpecific;
            componentConfig.DataType = command.DataType;

            await Context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
