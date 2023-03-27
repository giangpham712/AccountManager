using System.Collections.Generic;
using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Accounts.Queries.GetAllMachinesForAccount
{
    public class GetAllMachinesForAccountQuery : IRequest<IEnumerable<MachineDto>>
    {
        public long? Id { get; set; }
        public string UrlFriendlyName { get; set; }
    }
}