using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AccountManager.Application.Machines.Queries.GetSiteServerStatusForMachine
{
    public class GetSiteServerStatusForMachineQuery : IRequest<SiteServerStatusDto>
    {
        public long Id { get; set; }
    }

    public class
        GetSiteServerStatusForMachineQueryHandler : IRequestHandler<GetSiteServerStatusForMachineQuery,
            SiteServerStatusDto>
    {
        private readonly ICloudStateDbContext _context;

        public GetSiteServerStatusForMachineQueryHandler(ICloudStateDbContext context)
        {
            _context = context;
        }

        public async Task<SiteServerStatusDto> Handle(GetSiteServerStatusForMachineQuery query,
            CancellationToken cancellationToken)
        {
            var machine = await _context.Set<Domain.Entities.Machine.Machine>()
                .Include(x => x.Account)
                .Include("Account.MachineConfig")
                .FirstOrDefaultAsync(x => x.Id == query.Id, cancellationToken);

            if (machine == null)
                throw new EntityNotFoundException(nameof(Domain.Entities.Machine.Machine), query.Id);

            var scheme = machine.Account.MachineConfig.EnableSsl ? "https" : "http";
            using (var httpClient = new HttpClient
                   {
                       BaseAddress =
                           new Uri(
                               $"{scheme}://{machine.SiteName}.{machine.Account.UrlFriendlyName}.planetassociates.net:8080")
                   })
            {
                var response = await httpClient.GetAsync("irm/rest/v1.14/server-status", cancellationToken);
                if (!response.IsSuccessStatusCode) throw new CommandException();

                var serverStatus =
                    JsonConvert.DeserializeObject<SiteServerStatusDto>(await response.Content.ReadAsStringAsync());

                return serverStatus;
            }
        }
    }

    public class SiteServerStatusDto
    {
        public string Status { get; set; }
        public bool Operational { get; set; }
        public bool MongoOk { get; set; }
        public bool InterServerCommOk { get; set; }
        public bool RabbitMqOk { get; set; }
        public bool LibraryImportsOk { get; set; }
        public bool DiskSpaceOk { get; set; }
    }
}