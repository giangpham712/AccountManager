using AccountManager.Application.Services;
using FluentValidation;

namespace AccountManager.Application.SoftwareUpdate
{
    public class UpdateSoftwareCommandValidatorBase<T> : AbstractValidator<T> where T : UpdateSoftwareCommandBase
    {
        protected readonly IBuildFileService BuildFileService;
        protected readonly ICloudStateDbContext Context;
        protected readonly ILibraryFileService LibraryFileService;

        protected UpdateSoftwareCommandValidatorBase(ICloudStateDbContext context,
            ILibraryFileService libraryFileService, IBuildFileService buildFileService)
        {
            Context = context;
            LibraryFileService = libraryFileService;
            BuildFileService = buildFileService;
        }
    }
}