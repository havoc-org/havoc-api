using System;
using System.Collections.Generic;

namespace Havoc_API.Models;

public partial class ProjectStatus
{
    public int ProjectStatusId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
