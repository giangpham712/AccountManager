using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Application.Softwares.Dtos;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AccountManager.Application.Softwares.Queries.GetSoftwareStatus
{
    public class GetSoftwareStatusQueryHandler : IRequestHandler<GetSoftwareStatusQuery, IEnumerable<SoftwareStatusDto>>
    {
        private readonly ICloudStateDbContext _context;

        public GetSoftwareStatusQueryHandler(ICloudStateDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SoftwareStatusDto>> Handle(GetSoftwareStatusQuery request,
            CancellationToken cancellationToken)
        {
            var account = await _context.Set<Account>()
                .FirstOrDefaultAsync(x => x.Id == request.AccountId, cancellationToken);

            if (account == null) 
                throw new EntityNotFoundException(nameof(Account), request.AccountId);

            try
            {
                using var httpClient = new HttpClient
                {
                    BaseAddress = new Uri($"http://{account.UrlFriendlyName}.planetassociates.net:8081/deployer/v2/")
                };

                var response = await httpClient.GetAsync("maintenance", cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    return new List<SoftwareStatusDto>();
                }

                var json = await response.Content.ReadAsStringAsync();
                var maintenance = JsonConvert.DeserializeObject<GetMaintenanceResponse>(json, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

                return maintenance?.ServicesInfo.Select(x => new SoftwareStatusDto
                {
                    Name = x.Software,
                    Status = x.State
                });

            }
            catch (Exception e)
            {
                return new List<SoftwareStatusDto>();
            }
        }
    }

    public class GetMaintenanceResponse
    {
        public List<ServiceInfo> ServicesInfo { get; set; }
    }

    public class ServiceInfo
    {
        public string Software { get; set; }
        public string State { get; set; }
    }
}