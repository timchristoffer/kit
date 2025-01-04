public class Project : BaseEntity
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<UploadedFile> UploadedFiles { get; set; } = new List<UploadedFile>();
    public Guid UserId { get; set; } // Koppling till användare
    public User User { get; set; } = null!;
}