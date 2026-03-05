using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class DisputeImage
{
    public int Id { get; set; }

    public string FileName { get; set; } = null!;

    public string? FileDescription { get; set; }

    public string? FileExtension { get; set; }

    public long? FileSizeInBytes { get; set; }

    public string FilePath { get; set; } = null!;

    public int DisputeId { get; set; }

    public virtual Dispute Dispute { get; set; } = null!;
}
