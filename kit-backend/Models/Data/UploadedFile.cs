public class UploadedFile : BaseEntity
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DateTime UploadDate { get; set; } = DateTime.UtcNow;
    public long FileSize { get; set; }
    public string? Uploader { get; set; }
}