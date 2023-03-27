using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Common.Extensions;
using AccountManager.Domain.Entities.Machine;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Machines.Queries.GetDeployerLogForOperation
{
    public class GetDeployerLogForOperationQueryHandler : IRequestHandler<GetDeployerLogForOperationQuery, string>
    {
        private readonly ICloudStateDbContext _context;

        public GetDeployerLogForOperationQueryHandler(ICloudStateDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(GetDeployerLogForOperationQuery request, CancellationToken cancellationToken)
        {
            var operation = await _context.Set<Operation>()
                .Select(x => new { 
                    x.Id,
                    x.DeployerLog,
                    x.Machine
                })
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (operation == null)
            {
                throw new EntityNotFoundException(nameof(Operation), request.Id);
            }

            var deployerLogFileName = operation.DeployerLog;
            if (deployerLogFileName.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }

            var machine = operation.Machine;

            var deployerLogUrl =
                $"http://{(machine.IsLauncher ? machine.LauncherUrl : machine.SiteMasterUrl)}:8081/deployer/v2/log/{deployerLogFileName}";

            string content;
            try
            {
                var httpClient = new HttpClient();
                content = await httpClient.GetStringAsync(deployerLogUrl);
            }
            catch
            {
                content = string.Empty;
            }

            return content;
        }
    }
}