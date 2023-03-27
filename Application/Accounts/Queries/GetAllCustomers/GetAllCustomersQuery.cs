using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Accounts.Queries.GetAllCustomers
{
    public class GetAllCustomersQuery : IRequest<IEnumerable<string>>
    {
    }

    public class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, IEnumerable<string>>
    {
        private readonly ICloudStateDbContext _context;

        public GetAllCustomersQueryHandler(ICloudStateDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<string>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
        {
            var customers = await _context.Set<Account>()
                .Where(x => x.Customer != null && x.Customer.Trim() != string.Empty).Distinct().Select(x => x.Customer)
                .ToListAsync(cancellationToken);

            return customers;
        }
    }
}