namespace backend.DTOs
{
    public class ImageDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public string FileExtension { get; set; } = null!;
        public string? FileDescription { get; set; }
        public long FileSizeInBytes { get; set; }
    }
}
