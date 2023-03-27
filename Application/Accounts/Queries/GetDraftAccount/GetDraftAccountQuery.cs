using System;
using System.Collections.Generic;
using System.Text;
using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Accounts.Queries.GetDraftAccount
{
    public class GetDraftAccountQuery : IRequest<DraftAccountDto>
    {
        public Guid Id { get; set; }
    }
}
