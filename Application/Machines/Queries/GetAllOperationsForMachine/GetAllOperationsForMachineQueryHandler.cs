using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities.Machine;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Machines.Queries.GetAllOperationsForMachine
{
    public class
        GetAllOperationsForMachineQueryHandler : IRequestHandler<GetAllOperationsForMachineQuery,
            PagedResult<OperationDto>>
    {
        private readonly IMapper _mapper;
        private readonly ICloudStateDbContext _context;

        public GetAllOperationsForMachineQueryHandler(ICloudStateDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResult<OperationDto>> Handle(GetAllOperationsForMachineQuery request,
            CancellationToken cancellationToken)
        {
            var operationsQuery = _context.Set<Operation>()
                .AsNoTracking()
                .AsQueryable();

            if (request.Id.HasValue)
            {
                operationsQuery = operationsQuery.Where(x => x.MachineId == request.Id);
            }

            var total = await operationsQuery.CountAsync(cancellationToken);

            var operations = await operationsQuery.OrderByDescending(x => x.Timestamp)
                .Skip(request.StartIndex)
                .Take(request.Limit)
                .Select(x => new OperationDto
                {
                    Id = x.Id,
                    Timestamp = x.Timestamp,
                    FinishTime = x.FinishTime,
                    FailCounter = x.FailCounter,
                    Active = x.Active,
                    Status = x.Status,
                    OperationMode = x.OperationMode,
                    MachineName = x.Machine.Name,
                    MachineClassName = x.Machine.Class.Name,
                    Timeout = x.Type.Timeout,
                    OperationTypeName = x.Type.Name,
                    OperationTypeDescription = x.Type.Description,
                })
                .ToListAsync(cancellationToken);

            if (operations.Count == 0)
            {
                var firstOperationTimestamp = await operationsQuery.MinAsync(x => x.Timestamp, cancellationToken);
                var userOperations = await _context.Set<UserOperation>()
                    .Include(x => x.Machine).ThenInclude(x => x.Class)
                    .Include(x => x.Type)
                    .Where(x => x.MachineId == request.Id && x.Timestamp <= firstOperationTimestamp)
                    .OrderByDescending(x => x.Timestamp)
                    .ToListAsync(cancellationToken);

                return new PagedResult<OperationDto>
                {
                    Items = _mapper.Map<IEnumerable<OperationDto>>(userOperations),
                    TotalItems = total,
                    StartIndex = request.StartIndex,
                    Limit = request.Limit,
                    HasMore = false
                };
            }
            else
            {
                var fromTimestamp = operations.Min(x => x.Timestamp);
                var toTimestamp = operations.Max(x => x.Timestamp);

                var userOperationsQuery = _context.Set<UserOperation>()
                    .AsNoTracking()
                    .Include(x => x.Machine).ThenInclude(x => x.Class)
                    .Include(x => x.Type)
                    .Where(x => x.Timestamp >= fromTimestamp && x.Timestamp < toTimestamp);

                if (request.Id.HasValue)
                {
                    userOperationsQuery = userOperationsQuery.Where(x => x.MachineId == request.Id);
                }

                var userOperations = await userOperationsQuery
                    .OrderByDescending(x => x.Timestamp)
                    .ToListAsync(cancellationToken);

                var allOperations = operations
                    .Union(_mapper.Map<IEnumerable<OperationDto>>(userOperations))
                    .OrderByDescending(x => x.Timestamp)
                    .ToList();

                return new PagedResult<OperationDto>
                {
                    Items = allOperations,
                    TotalItems = total,
                    StartIndex = request.StartIndex,
                    Limit = request.Limit,
                    HasMore = allOperations.Count > 0
                };
            }
        }
    }
}