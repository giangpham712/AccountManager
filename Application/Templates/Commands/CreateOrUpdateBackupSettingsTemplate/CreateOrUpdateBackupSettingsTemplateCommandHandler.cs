using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AutoMapper;
using MediatR;

namespace AccountManager.Application.Templates.Commands.CreateOrUpdateBackupSettingsTemplate
{
    public class
        CreateOrUpdateBackupSettingsTemplateCommandHandler : CommandHandlerBase<
            CreateOrUpdateBackupSettingsTemplateCommand, long>
    {
        private readonly IMapper _mapper;
        public CreateOrUpdateBackupSettingsTemplateCommandHandler(IMediator mediator, ICloudStateDbContext context, IMapper mapper)
            : base(mediator, context)
        {
            _mapper = mapper;
        }

        public override async Task<long> Handle(CreateOrUpdateBackupSettingsTemplateCommand command,
            CancellationToken cancellationToken)
        {
            BackupConfig template;
            if (command.Id > 0)
            {
                template = await Context.Set<BackupConfig>()
                    .FirstOrDefaultAsync(x => x.IsTemplate && x.Id == command.Id, cancellationToken);

                if (template == null)
                    throw new EntityNotFoundException(nameof(BackupConfig), command.Id);
            }
            else
            {
                template = new BackupConfig { IsTemplate = true };
                Context.Set<BackupConfig>().Add(template);
            }

            _mapper.Map(command, template);
            template.Creator = command.User;

            await Context.SaveChangesAsync(cancellationToken);

            return template.Id;
        }
    }
}