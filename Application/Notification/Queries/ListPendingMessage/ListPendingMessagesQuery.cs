using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Machine;
using MediatR;

namespace AccountManager.Application.Notification.Queries.ListPendingMessage
{
    public class ListPendingMessagesQuery : IRequest<IEnumerable<Message>>
    {
    }

    public class ListPendingMessagesQueryHandler : IRequestHandler<ListPendingMessagesQuery, IEnumerable<Message>>
    {
        private readonly ICloudStateDbContext _context;

        public ListPendingMessagesQueryHandler(ICloudStateDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Message>> Handle(ListPendingMessagesQuery request,
            CancellationToken cancellationToken)
        {
            return await _context.Set<Message>()
                .Include(x => x.Machine)
                .OrderBy(x => x.Timestamp).ToListAsync(cancellationToken);
        }
    }
}