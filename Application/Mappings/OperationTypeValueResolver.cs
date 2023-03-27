using System;
using System.Collections.Generic;
using System.Linq;
using AccountManager.Application.Caching;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Machine;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace AccountManager.Application.Mappings
{
    public class OperationTypeValueResolver : IValueResolver<Operation, OperationDto, OperationTypeDto>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMapper _mapper;

        public OperationTypeValueResolver(IMapper mapper, IServiceProvider serviceProvider)
        {
            _mapper = mapper;
            _serviceProvider = serviceProvider;
        }

        public OperationTypeDto Resolve(Operation source, OperationDto destination, OperationTypeDto destMember,
            ResolutionContext context)
        {
            var dbContext = _serviceProvider.GetService<ICloudStateDbContext>();
            var cacheManager = _serviceProvider.GetService<ICacheManager>();

            var operationTypes =
                cacheManager.Get<IEnumerable<OperationType>>("OperationTypes",
                    () => dbContext.Set<OperationType>().ToList());
            var operationTypeMapByName = operationTypes.ToDictionary(x => x.Name, x => x);

            OperationType type;

            return operationTypeMapByName.TryGetValue(source.TypeName, out type)
                ? _mapper.Map<OperationTypeDto>(type)
                : new OperationTypeDto();
        }
    }
}