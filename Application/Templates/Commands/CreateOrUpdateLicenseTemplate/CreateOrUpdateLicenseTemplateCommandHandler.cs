using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AutoMapper;
using MediatR;

namespace AccountManager.Application.Templates.Commands.CreateOrUpdateLicenseTemplate
{
    public class
        CreateOrUpdateLicenseTemplateCommandHandler : CommandHandlerBase<CreateOrUpdateLicenseTemplateCommand, long>
    {
        private readonly IMapper _mapper;
        public CreateOrUpdateLicenseTemplateCommandHandler(IMediator mediator, ICloudStateDbContext context, IMapper mapper) : base(
            mediator, context)
        {
            _mapper = mapper;
        }

        public override async Task<long> Handle(CreateOrUpdateLicenseTemplateCommand command,
            CancellationToken cancellationToken)
        {
            LicenseConfig template;
            if (command.Id > 0)
            {
                template = await Context.Set<LicenseConfig>()
                    .FirstOrDefaultAsync(x => x.IsTemplate && x.Id == command.Id, cancellationToken);

                if (template == null)
                    throw new EntityNotFoundException(nameof(LicenseConfig), command.Id);
            }
            else
            {
                template = new LicenseConfig { IsTemplate = true };
                Context.Set<LicenseConfig>().Add(template);
            }

            _mapper.Map(command, template);
            template.Creator = command.User;
            await Context.SaveChangesAsync(cancellationToken);

            return template.Id;
        }
    }
}