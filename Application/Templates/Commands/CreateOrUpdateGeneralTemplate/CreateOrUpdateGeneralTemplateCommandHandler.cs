using System;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AutoMapper;
using MediatR;

namespace AccountManager.Application.Templates.Commands.CreateOrUpdateGeneralTemplate
{
    public class
        CreateOrUpdateGeneralTemplateCommandHandler : CommandHandlerBase<CreateOrUpdateGeneralTemplateCommand, long>
    {
        private readonly IMapper _mapper;

        public CreateOrUpdateGeneralTemplateCommandHandler(
            IMediator mediator, 
            ICloudStateDbContext context, 
            IMapper mapper) : base(
            mediator, context)
        {
            _mapper = mapper;
        }

        public override async Task<long> Handle(CreateOrUpdateGeneralTemplateCommand command,
            CancellationToken cancellationToken)
        {
            var transaction = Context.GetTransaction();
            try
            {
                Account accountTemplate;
                if (command.Id > 0)
                    accountTemplate = await UpdateTemplate(command, cancellationToken);
                else
                    accountTemplate = await CreateTemplate(command, cancellationToken);

                await Context.SaveChangesAsync(cancellationToken);
                transaction.Commit();
                return accountTemplate.Id;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        private async Task<Account> CreateTemplate(CreateOrUpdateGeneralTemplateCommand command,
            CancellationToken cancellationToken)
        {
            var accountTemplate = new Account { IsTemplate = true };
            _mapper.Map(command, accountTemplate);

            var instanceSettingsTemplate = new MachineConfig { IsTemplate = true };
            _mapper.Map(command, instanceSettingsTemplate);
            accountTemplate.MachineConfig = instanceSettingsTemplate;

            var licenseTemplate = new LicenseConfig { IsTemplate = true };
            _mapper.Map(command, licenseTemplate);
            accountTemplate.LicenseConfig = licenseTemplate;

            var backupSettingsTemplate = new BackupConfig { IsTemplate = true };
            _mapper.Map(command, backupSettingsTemplate);
            accountTemplate.BackupConfig = backupSettingsTemplate;

            Context.Set<Account>().Add(accountTemplate);
            return accountTemplate;
        }

        private async Task<Account> UpdateTemplate(CreateOrUpdateGeneralTemplateCommand command,
            CancellationToken cancellationToken)
        {
            var accountTemplate = await Context.Set<Account>()
                .Include(x => x.Billing)
                .Include(x => x.Contact)
                .Include(x => x.LicenseConfig)
                .Include(x => x.MachineConfig)
                .Include(x => x.BackupConfig)
                .FirstOrDefaultAsync(x => x.IsTemplate && x.Id == command.Id, cancellationToken);

            if (accountTemplate == null)
                throw new EntityNotFoundException(nameof(Account), command.Id);

            var instanceSettingsTemplate = accountTemplate.MachineConfig;
            if (instanceSettingsTemplate != null)
            {
                _mapper.Map(command, instanceSettingsTemplate);
            }
            else
            {
                instanceSettingsTemplate = new MachineConfig { IsTemplate = true };
                _mapper.Map(command, instanceSettingsTemplate);
                accountTemplate.MachineConfig = instanceSettingsTemplate;
            }

            var licenseTemplate = accountTemplate.LicenseConfig;
            if (licenseTemplate != null)
            {
                _mapper.Map(command, licenseTemplate);
            }
            else
            {
                licenseTemplate = new LicenseConfig { IsTemplate = true };
                _mapper.Map(command, licenseTemplate);
                accountTemplate.LicenseConfig = licenseTemplate;
            }

            var backupSettingsTemplate = accountTemplate.BackupConfig;
            if (backupSettingsTemplate != null)
            {
                _mapper.Map(command, backupSettingsTemplate);
            }
            else
            {
                backupSettingsTemplate = new BackupConfig { IsTemplate = true };
                _mapper.Map(command, backupSettingsTemplate);
                accountTemplate.BackupConfig = backupSettingsTemplate;
            }

            _mapper.Map(command, accountTemplate);

            return accountTemplate;
        }
    }
}