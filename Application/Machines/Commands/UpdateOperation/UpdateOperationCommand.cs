using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AccountManager.Common;

namespace AccountManager.Application.Machines.Commands.UpdateOperation
{
    public class UpdateOperationCommand : CommandBase
    {
        static UpdateOperationCommand()
        {
            Patchables = typeof(UpdateOperationCommand).GetProperties().Where(x => x.PropertyType.IsGenericType &&
                x.PropertyType.GetGenericTypeDefinition() ==
                typeof(Patch<>));
        }

        public static IEnumerable<PropertyInfo> Patchables { get; }

        public long MachineId { get; set; }
        public long OperationId { get; set; }

        public Patch<string> Params { get; set; }
    }
}