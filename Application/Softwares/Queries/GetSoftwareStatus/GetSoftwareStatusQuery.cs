using System.Collections.Generic;
using AccountManager.Application.Softwares.Dtos;
using MediatR;

namespace AccountManager.Application.Softwares.Queries.GetSoftwareStatus
{
    public class GetSoftwareStatusQuery : IRequest<IEnumerable<SoftwareStatusDto>>
    {
        public long AccountId { get; set; }
    }
}