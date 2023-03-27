using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Application.Models.Dto;
using AccountManager.Common.Extensions;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AccountManager.Domain.Extensions;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AccountManager.Application.Accounts.Queries.GetLauncherLoginsForAccount
{
    public class GetLauncherLoginsForAccountQuery :  IRequest<IEnumerable<LauncherLoginDto>>
    {
        public long AccountId { get; set; }
    }

    public class
        GetLauncherLoginsForAccountQueryHandler : IRequestHandler<GetLauncherLoginsForAccountQuery,
            IEnumerable<LauncherLoginDto>>
    {
        private readonly ICloudStateDbContext _context;
        private readonly IMapper _mapper;

        public GetLauncherLoginsForAccountQueryHandler(ICloudStateDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LauncherLoginDto>> Handle(GetLauncherLoginsForAccountQuery query,
            CancellationToken cancellationToken)
        {
            var account = await _context.Set<Account>()
                .AsNoTracking()
                .Include(x => x.Keys)
                .Where(x => x.Id == query.AccountId)
                .FirstOrDefaultAsync(cancellationToken);

            if (account == null)
                throw new EntityNotFoundException(nameof(Account), query.AccountId);

            if (account.Keys.LauncherUsers.IsNullOrWhiteSpace())
            {
                return new List<LauncherLoginDto>();
            }

            var launcherLogins = JsonConvert.DeserializeObject<List<LauncherUser>>(account.Keys.LauncherUsers);
            return _mapper.Map<IEnumerable<LauncherLoginDto>>(launcherLogins);
        }
    }
}
