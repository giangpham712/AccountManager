namespace AccountManager.Application.Models.Dto
{
    public class OperationTypeDto
    {
        public string Name { get; set; }
        public long RetryAttempts { get; set; }
        public long Timeout { get; set; }
        public string Fallback { get; set; }
        public string Description { get; set; }
        public bool? RequiresMaintMode { get; set; }
        public bool? CanBeManual { get; set; }
    }
}