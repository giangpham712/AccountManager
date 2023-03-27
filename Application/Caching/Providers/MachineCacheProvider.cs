using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Machine;

namespace AccountManager.Application.Caching.Providers
{
    public class MachineCacheProvider : IMachineCacheProvider
    {
        public Task<IEnumerable<OperationType>> ListOperationTypes()
        {
            throw new NotImplementedException();
        }
    }
}