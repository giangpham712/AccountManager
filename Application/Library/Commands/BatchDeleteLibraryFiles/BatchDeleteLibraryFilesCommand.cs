using System;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Library.Commands.DeleteLibraryFile;
using MediatR;

namespace AccountManager.Application.Library.Commands.BatchDeleteLibraryFiles
{
    public class BatchDeleteLibraryFilesCommand : CommandBase
    {
        public long[] Ids { get; set; }
    }

    public class BatchDeleteLibraryFilesCommandHandler : IRequestHandler<BatchDeleteLibraryFilesCommand>
    {
        private readonly ICloudStateDbContext _context;
        private readonly IMediator _mediator;

        public BatchDeleteLibraryFilesCommandHandler(ICloudStateDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(BatchDeleteLibraryFilesCommand request, CancellationToken cancellationToken)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                foreach (var id in request.Ids)
                {
                    var deleteFileCommand = new DeleteLibraryFileCommand { Id = id };
                    await _mediator.Send(deleteFileCommand, cancellationToken);
                }

                await transaction.CommitAsync(cancellationToken);
                return Unit.Value;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}