using System;
using System.Collections.Generic;

namespace Havoc_API.Models;

public partial class Task
{
    public int TaskId { get; set; }

    public int ProjectId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? Start { get; set; }

    public DateTime? Deadline { get; set; }

    public int CreatorId { get; set; }

    public int TaskStatusId { get; set; }

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual User Creator { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;

    public virtual TaskStatus TaskStatus { get; set; } = null!;

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
