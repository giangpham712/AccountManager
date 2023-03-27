using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AccountManager.Application.Caching;
using AccountManager.Domain.Entities.Account;

namespace AccountManager.Application.Services
{
    public class DraftAccountService : IDraftAccountService
    {
        private readonly ICacheManager _cacheManager;

        public DraftAccountService(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public Guid CreateDraftAccount(Account account)
        {
            var guid = Guid.NewGuid();
            _cacheManager.Set($"DraftAccount:{guid}", account, 60, true);
            return guid;
        }

        public void DeleteDraftAccount(Guid guid)
        {
            _cacheManager.Remove($"DraftAccount:{guid}");
        }

        public Account GetDraftAccount(Guid guid)
        {
            var draftAccount = _cacheManager.Get<Account>($"DraftAccount:{guid}");
            return draftAccount;
        }
    }

    public interface IDraftAccountService
    {
        Guid CreateDraftAccount(Account account);
        void DeleteDraftAccount(Guid guid);
        Account GetDraftAccount(Guid guid);
    }
    
}
