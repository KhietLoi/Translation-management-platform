    namespace MySolution.Domain.Entities;

    public class TranslationRelease
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public int Version { get; set; }
        public string BlobFileName { get; set; } = null!;
        public string? DownloadUrl { get; set; }
        public string? Checksum { get; set; }
        public int TotalKey {get; set;}
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
        public Guid PublishedBy { get; set; }
        public DateTime PublishedAt { get; set; }
        public Project Project { get; set; } = null!;
    }