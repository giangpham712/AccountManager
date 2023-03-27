using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Domain.Entities.Machine;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Machines.Queries.GetOutputForOperation
{
    public class GetOutputForOperationQueryHandler : IRequestHandler<GetOutputForOperationQuery, string>
    {
        private readonly ICloudStateDbContext _context;

        public GetOutputForOperationQueryHandler(ICloudStateDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(GetOutputForOperationQuery request, CancellationToken cancellationToken)
        {
            var operation = await _context.Set<Operation>()
                .Select(x => new { 
                    x.Id,
                    x.Output

                })
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (operation == null)
            {
                throw new EntityNotFoundException(nameof(Operation), request.Id);
            }

            return operation.Output;
        }
    }
}