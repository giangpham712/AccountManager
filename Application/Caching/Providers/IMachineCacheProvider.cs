using System.Collections.Generic;
using System.Threading.Tasks;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Machine;

namespace AccountManager.Application.Caching.Providers
{
    public interface IMachineCacheProvider
    {
        Task<IEnumerable<OperationType>> ListOperationTypes();
    }
}