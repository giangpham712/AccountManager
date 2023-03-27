using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Application.Models.Dto.Saas;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AutoMapper;
using MediatR;

namespace AccountManager.Application.Saas.Queries.GetAccount
{
    public class GetAccountQueryHandler : IRequestHandler<GetAccountQuery, SaasAccountDto>
    {
        private readonly IMapper _mapper;
        private readonly ICloudStateDbContext _context;

        public GetAccountQueryHandler(ICloudStateDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<SaasAccountDto> Handle(GetAccountQuery request, CancellationToken cancellationToken)
        {
            var account = await _context.Set<Account>()
                .Include(x => x.LicenseConfig)
                .Include(x => x.MachineConfig)
                .Where(x => x.UrlFriendlyName == request.UrlFriendlyName)
                .FirstOrDefaultAsync(cancellationToken);

            if (account == null)
                throw new EntityNotFoundException(nameof(Account), request.UrlFriendlyName);

            var accountDto = _mapper.Map<SaasAccountDto>(account);
            accountDto.MarketingVersion = account.MachineConfig.MarketingVersion;


            return await Task.FromResult(accountDto);
        }
    }
}