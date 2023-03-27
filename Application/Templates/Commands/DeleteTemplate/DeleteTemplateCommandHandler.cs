using System;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using MediatR;

namespace AccountManager.Application.Templates.Commands.DeleteTemplate
{
    public class DeleteTemplateCommandHandler : IRequestHandler<DeleteTemplateCommand, Unit>
    {
        private readonly ICloudStateDbContext _context;

        public DeleteTemplateCommandHandler(ICloudStateDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteTemplateCommand request, CancellationToken cancellationToken)
        {
            switch (request.Type)
            {
                case "general":
                    await DeleteGeneralTemplate(request.Id, cancellationToken);
                    break;
                case "instance-settings":
                    await DeleteTemplate<MachineConfig>(request.Id, cancellationToken);
                    break;
                case "license-settings":
                    await DeleteTemplate<LicenseConfig>(request.Id, cancellationToken);
                    break;
                case "backup-settings":
                    await DeleteTemplate<BackupConfig>(request.Id, cancellationToken);
                    break;
                default:
                    throw new Exception("Invalid template type");
            }

            return await Task.FromResult(Unit.Value);
        }

        public async Task DeleteTemplate<T>(long id, CancellationToken cancellationToken)
            where T : class, ISupportTemplate
        {
            var template = await _context.Set<T>()
                .FirstOrDefaultAsync(x => x.Id == id && x.IsTemplate, cancellationToken);

            if (template == null)
                throw new EntityNotFoundException("Template", id);

            _context.Set<T>().Remove(template);

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteGeneralTemplate(long id, CancellationToken cancellationToken)
        {
            using (var transaction = _context.GetTransaction())
            {
                try
                {
                    var template = await _context.Set<Account>()
                        .Include(x => x.Contact)
                        .Include(x => x.Billing)
                        .Include(x => x.LicenseConfig)
                        .Include(x => x.MachineConfig)
                        .Include(x => x.BackupConfig)
                        .FirstOrDefaultAsync(x => x.Id == id && x.IsTemplate, cancellationToken);

                    if (template == null)
                        throw new EntityNotFoundException("General template", id);


                    _context.Set<Contact>().Remove(template.Contact);
                    _context.Set<Billing>().Remove(template.Billing);

                    if (template.MachineConfig != null) _context.Set<MachineConfig>().Remove(template.MachineConfig);

                    if (template.LicenseConfig != null) _context.Set<LicenseConfig>().Remove(template.LicenseConfig);

                    if (template.BackupConfig != null) _context.Set<BackupConfig>().Remove(template.BackupConfig);

                    _context.Set<Account>().Remove(template);

                    await _context.SaveChangesAsync(cancellationToken);
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}