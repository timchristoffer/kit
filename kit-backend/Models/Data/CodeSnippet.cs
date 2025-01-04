public class CodeSnippet : BaseEntity
{
    public Guid SnippetId { get; set; } // Unikt ID för varje kodblock
    public string Code { get; set; } = string.Empty; // Själva koden
}