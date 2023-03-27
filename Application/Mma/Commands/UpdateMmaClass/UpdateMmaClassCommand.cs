using AccountManager.Application.Models.Dto;

namespace AccountManager.Application.Mma.Commands.UpdateMmaClass
{
    public class UpdateMmaClassCommand : CommandBase<ClassDto>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string DeployerBranch { get; set; }
        public string DeployerVersion { get; set; }
        public bool Standby { get; set; }
        public int PriorityGroup { get; set; }
    }
}