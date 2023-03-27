using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Application.Services;
using AutoMapper;
using MediatR;

namespace AccountManager.Application.Accounts.Queries.GetDraftAccount
{
    public class GetDraftAccountQueryHandler : IRequestHandler<GetDraftAccountQuery, DraftAccountDto>
    {
        private readonly IMapper _mapper;
        private readonly IDraftAccountService _draftAccountService;

        public GetDraftAccountQueryHandler(IDraftAccountService draftAccountService, IMapper mapper)
        {
            _draftAccountService = draftAccountService;
            _mapper = mapper;
        }

        public async Task<DraftAccountDto> Handle(GetDraftAccountQuery request, CancellationToken cancellationToken)
        {
            var draftAccount = _draftAccountService.GetDraftAccount(request.Id);

            var draftAccountDto = _mapper.Map<DraftAccountDto>(draftAccount);
            draftAccountDto.DraftId = request.Id;

            return draftAccountDto;
        }
    }
}
