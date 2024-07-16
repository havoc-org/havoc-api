using System;
using System.Collections.Generic;

namespace Havoc_API.Models;

public partial class Attachment
{
    public int AttachmentId { get; set; }

    public int UserId { get; set; }

    public int TaskId { get; set; }

    public string FileLink { get; set; } = null!;

    public virtual Task Task { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
