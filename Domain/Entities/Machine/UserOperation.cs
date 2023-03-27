using System;

namespace AccountManager.Domain.Entities.Machine
{
    public class UserOperation : IEntity
    {
        public long MachineId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string Params { get; set; }
        public string TypeName { get; set; }
        public string User { get; set; }
        public string Output { get; set; }

        public Machine Machine { get; set; }
        public UserOperationType Type { get; set; }
        public long Id { get; set; }
    }

    public static class UserOperationTypes
    {
        public static string CreateAccount = "CRTEACC";

        public static string Backup = "FORCEBACK";
        public static string Restore = "RESTBACK";

        public static string PushLicenseSettings = "PUSHLIC";
        public static string PushInstanceSettings = "PUSHSOFT";
        public static string PushAccountProperties = "PUSHACCT";
        public static string PushBackupSettings = "PUSHBKUP";
        public static string PushIdleSchedule = "PUSHIDLE";

        public static string StartMachine = "STRTMAC";
        public static string StopMachine = "STOPMAC";
        public static string TerminateMachine = "TERMMAC";

        public static string ResetOperation = "RESETOP";
        public static string QueueOperation = "QUEUEOP";
        public static string ForcePopulate = "FORCEPOP";
        public static string PublishAsSampleData = "PUBSAMPLEDAT";

        public static string UpdateMachine = "UPDTMAC";
        public static string RecreateMachine = "RECREATE";

        public static string ChangeInstanceType = "INSTTYPE";
    }
}