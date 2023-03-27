using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Accounts.Extensions;
using AccountManager.Application.Exceptions;
using AccountManager.Application.License;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Accounts.Commands.UpdateLicenseSettings
{
    public class UpdateLicenseSettingsCommandHandler : CommandHandlerBase<UpdateLicenseSettingsCommand, Unit>
    {
        private readonly ILicenseGenerator _licenseGenerator;
        private readonly IMapper _mapper;

        public UpdateLicenseSettingsCommandHandler(IMediator mediator, ICloudStateDbContext context, IMapper mapper, ILicenseGenerator licenseGenerator) : base(
            mediator, context)
        {
            _mapper = mapper;
            _licenseGenerator = licenseGenerator;
        }

        public override async Task<Unit> Handle(UpdateLicenseSettingsCommand command,
            CancellationToken cancellationToken)
        {
            var account = await Context.Set<Account>()
                .Include(x => x.Keys)
                .Include(x => x.Machines)
                .Include(x => x.LicenseConfig)
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == command.AccountId, cancellationToken);

            if (account == null)
                throw new EntityNotFoundException(nameof(Account), command.AccountId);


            _mapper.Map(command, account.LicenseConfig);

            var parentAccount = account;
            if (account.ParentId.HasValue)
                parentAccount = await Context.Set<Account>()
                    .Include(x => x.Keys)
                    .Include(x => x.LicenseConfig)
                    .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == account.ParentId.Value, cancellationToken);

            if (parentAccount != null)
            {
                var addOnAccounts = await Context.Set<Account>()
                    .Include(x => x.Keys)
                    .Include(x => x.LicenseConfig)
                    .Where(x => x.ParentId == parentAccount.Id)
                    .ToListAsync(cancellationToken);

                var addOnLicenseConfigs = addOnAccounts.Select(x => x.LicenseConfig).ToList();

                var keys = parentAccount.Keys;


                // Update license file for parent account
                var licenseBytes = _licenseGenerator.GenerateLicense(parentAccount.LicenseConfig, addOnLicenseConfigs,
                    keys.LicensePrivate);

                parentAccount.License = Convert.ToBase64String(licenseBytes);
                await parentAccount.SetLastUserCycle(Context);

                var launcherMachine = parentAccount.Machines.FirstOrDefault(x => x.IsLauncher);
                if (launcherMachine != null)
                {
                    launcherMachine.SetOperationModeToNormal();
                    launcherMachine.Turbo = true;
                }
            }

            await Context.SaveChangesAsync(cancellationToken, out var changes);

            // Add audit log
            await Mediator.Publish(new LicenseSettingsPushedEvent
            {
                User = command.User,
                Account = account,
                Command = command,
                Changes = changes
            }, cancellationToken);

            return Unit.Value;
        }
    }
}