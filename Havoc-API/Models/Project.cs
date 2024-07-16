using System;
using System.Collections.Generic;

namespace Havoc_API.Models;

public partial class Project
{
    public int ProjectId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public byte[]? Background { get; set; }

    public int CreatorId { get; set; }

    public DateTime? Start { get; set; }

    public DateTime? Deadline { get; set; }

    public DateTime LastModified { get; set; }

    public int ProjectStatusId { get; set; }

    public virtual User Creator { get; set; } = null!;

    public virtual ICollection<Participation> Participations { get; set; } = new List<Participation>();

    public virtual ProjectStatus ProjectStatus { get; set; } = null!;

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
