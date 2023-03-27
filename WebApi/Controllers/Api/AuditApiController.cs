using System.Collections.Generic;
using System.Threading.Tasks;
using AccountManager.Application.Audit.Queries.GetAuditLogs;
using AccountManager.Domain;
using AccountManager.Domain.Entities.Audit;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountManager.WebApi.Controllers.Api
{
    [Route(ApiVersion + "/audit")]
    public class AuditApiController : AuthorizedApiController
    {
        public AuditApiController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        [Route("logs")]
        public async Task<IEnumerable<AuditLog>> GetLogs(
            [FromQuery] long? accountId = null,
            [FromQuery] string action = null,
            [FromQuery] int startIndex = 0,
            [FromQuery] int limit = 10,
            [FromQuery] SortDirection sortDir = SortDirection.Descending)
        {
            var auditLogs = await Mediator.Send(new GetAuditLogsQuery
            {
                AccountId = accountId,
                Action = action,
                StartIndex = startIndex,
                Limit = limit,
                SortDirection = sortDir
            });

            return auditLogs;
        }
    }
}