public class ErrorLog : BaseEntity
{
    public Guid LogId { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public string StackTrace { get; set; } = string.Empty;
    public string AdditionalInfo { get; set; } = string.Empty;
}