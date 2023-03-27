using MediatR;

namespace AccountManager.Application.Machines.Queries.GetDeployerLogForOperation
{
    public class GetDeployerLogForOperationQuery : IRequest<string>
    {
        public long Id { get; set; }
    }
}
