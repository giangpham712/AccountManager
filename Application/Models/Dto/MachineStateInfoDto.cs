using System.Collections.Generic;

namespace AccountManager.Application.Models.Dto
{
    public class MachineStateInfoDto
    {
        public IEnumerable<StateDto> States { get; set; }
        public string BundleVersion { get; set; }
    }
}