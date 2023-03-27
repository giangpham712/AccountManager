using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AutoMapper;
using MediatR;

namespace AccountManager.Application.Templates.Commands.CreateOrUpdateInstanceSettingsTemplate
{
    public class
        CreateOrUpdateInstanceSettingsTemplateCommandHandler : CommandHandlerBase<
            CreateOrUpdateInstanceSettingsTemplateCommand, long>
    {
        private readonly IMapper _mapper;
        public CreateOrUpdateInstanceSettingsTemplateCommandHandler(IMediator mediator,
            ICloudStateDbContext context, IMapper mapper) : base(mediator, context)
        {
            _mapper = mapper;
        }

        public override async Task<long> Handle(CreateOrUpdateInstanceSettingsTemplateCommand command,
            CancellationToken cancellationToken)
        {
            MachineConfig template;
            if (command.Id > 0)
            {
                template = await Context.Set<MachineConfig>()
                    .FirstOrDefaultAsync(x => x.IsTemplate && x.Id == command.Id, cancellationToken);

                if (template == null)
                    throw new EntityNotFoundException(nameof(MachineConfig), command.Id);
            }
            else
            {
                template = new MachineConfig { IsTemplate = true };
                Context.Set<MachineConfig>().Add(template);
            }

            _mapper.Map(command, template);
            template.Creator = command.User;

            await Context.SaveChangesAsync(cancellationToken);

            return template.Id;
        }
    }
}