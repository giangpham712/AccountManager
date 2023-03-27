namespace AccountManager.Domain.Entities.Machine
{
    public class State : StateBase
    {
        public bool Desired { get; set; }
    }

    public class HistoricalDesiredState : StateBase
    {
    }
}