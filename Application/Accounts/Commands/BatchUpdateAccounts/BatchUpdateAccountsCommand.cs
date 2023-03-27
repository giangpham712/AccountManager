using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AccountManager.Common;

namespace AccountManager.Application.Accounts.Commands.BatchUpdateAccounts
{
    public class BatchUpdateAccountsCommand : CommandBase
    {
        static BatchUpdateAccountsCommand()
        {
            Patchables = typeof(BatchUpdateAccountsCommand).GetProperties().Where(x => x.PropertyType.IsGenericType &&
                x.PropertyType.GetGenericTypeDefinition() ==
                typeof(Patch<>));
        }

        public static IEnumerable<PropertyInfo> Patchables { get; }

        public long[] AccountIds { get; set; }

        public Patch<long?> ClassId { get; set; }
        public Patch<bool> Managed { get; set; }
        public Patch<bool> AutoTest { get; set; }
        public Patch<string> AutoTestBranch { get; set; }
    }
}