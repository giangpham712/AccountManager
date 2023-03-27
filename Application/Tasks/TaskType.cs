namespace AccountManager.Application.Tasks
{
    public enum TaskType
    {
        StartMachine,
        StopMachine,
        TerminateMachine,
        BackupMachine,
        RestoreMachineBackup,
        UpdateMachineSoftwares,
        ChangeMachineInstanceType
    }
}