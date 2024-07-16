using System;
using System.Collections.Generic;

namespace Havoc_API.Models;

public partial class Assignment
{
    public int UserId { get; set; }

    public int TaskId { get; set; }

    public string? Description { get; set; }

    public virtual Task Task { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
