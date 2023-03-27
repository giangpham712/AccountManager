using MediatR;

namespace AccountManager.Application.Machines.Queries.GetOutputForOperation
{
    public class GetOutputForOperationQuery : IRequest<string>
    {
        public long Id { get; set; }
    }
}
