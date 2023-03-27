using FluentValidation;

namespace AccountManager.Application.SoftwareUpdate.UpdateSoftwareForMachines
{
    public class UpdateSoftwareForMachinesCommandValidator : AbstractValidator<UpdateSoftwareForMachinesCommand>
    {
        private readonly ICloudStateDbContext _context;

        public UpdateSoftwareForMachinesCommandValidator(ICloudStateDbContext context)
        {
            _context = context;
        }
    }
}