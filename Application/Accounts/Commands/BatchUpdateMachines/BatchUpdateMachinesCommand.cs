using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AccountManager.Application.Accounts.Commands.UpdateMachine;
using AccountManager.Common;

namespace AccountManager.Application.Accounts.Commands.BatchUpdateMachines
{
    public class BatchUpdateMachinesCommand : CommandBase
    {
        static BatchUpdateMachinesCommand()
        {
            Patchables = typeof(UpdateMachineCommand).GetProperties().Where(x => x.PropertyType.IsGenericType &&
                x.PropertyType.GetGenericTypeDefinition() ==
                typeof(Patch<>));
        }

        public static IEnumerable<PropertyInfo> Patchables { get; }

        public long[] MachineIds { get; set; }
        public Patch<long?> ClassId { get; set; }
        public Patch<bool> Managed { get; set; }
        public Patch<bool?> ManualMaintenance { get; set; }
        public Patch<bool> Turbo { get; set; }
    }
}