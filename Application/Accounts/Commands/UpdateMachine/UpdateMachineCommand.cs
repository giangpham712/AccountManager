using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AccountManager.Common;

namespace AccountManager.Application.Accounts.Commands.UpdateMachine
{
    public class UpdateMachineCommand : CommandBase
    {
        static UpdateMachineCommand()
        {
            Patchables = typeof(UpdateMachineCommand).GetProperties().Where(x => x.PropertyType.IsGenericType &&
                x.PropertyType.GetGenericTypeDefinition() ==
                typeof(Patch<>));
        }

        public static IEnumerable<PropertyInfo> Patchables { get; }

        public long AccountId { get; set; }
        public long MachineId { get; set; }
        public Patch<long?> ClassId { get; set; }
        public Patch<bool> Managed { get; set; }
        public Patch<bool?> ManualMaintenance { get; set; }
        public Patch<bool> OverseeTermination { get; set; }
        public Patch<bool> Turbo { get; set; }
        public Patch<bool> RunSmokeTest { get; set; }
        public Patch<bool> SampleData { get; set; }
        public Patch<string> SampleDataFile { get; set; }
    }
}