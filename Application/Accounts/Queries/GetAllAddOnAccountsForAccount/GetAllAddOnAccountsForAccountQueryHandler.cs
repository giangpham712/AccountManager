using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Accounts.Queries.GetAllAddOnAccountsForAccount
{
    public class
        GetAllAddOnAccountsForAccountQueryHandler : IRequestHandler<GetAllAddOnAccountsForAccountQuery,
            IEnumerable<AccountDto>>
    {
        private readonly ICloudStateDbContext _context;
        private readonly IMapper _mapper;

        public GetAllAddOnAccountsForAccountQueryHandler(ICloudStateDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AccountDto>> Handle(GetAllAddOnAccountsForAccountQuery request,
            CancellationToken cancellationToken)
        {
            var addOnAccounts = await _context.Set<Account>()
                .Where(x => x.ParentId == request.Id)
                .ToListAsync(cancellationToken);

            var addOnAccountDtos = _mapper.Map<IEnumerable<AccountDto>>(addOnAccounts);

            return await Task.FromResult(addOnAccountDtos);
        }
    }
}