using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AccountManager.Domain.Entities.Public;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Accounts.Extensions
{
    public static class AccountExtensions
    {
        public static async Task SetLastUserCycle(this Account account, ICloudStateDbContext context)
        {
            var mmaInstance = await context.Set<MmaInstance>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MachineClassId == account.ClassId);

            if (mmaInstance == null) return;

            account.LastUserCycle = mmaInstance.MmaCycle ?? 0;
        }
    }
}