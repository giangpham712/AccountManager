using System.Collections.Generic;

namespace AccountManager.Application.SoftwareUpdate.UpdateSoftwareForMachines
{
    public class UpdateSoftwareForMachinesCommand : UpdateSoftwareCommandBase
    {
        public List<long> Machines { get; set; }

        public long Id { get; set; }
    }
}