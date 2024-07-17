using Havoc_API.Exceptions;
using System;
using System.Collections.Generic;

namespace Havoc_API.Models;

public partial class ProjectStatus
{
    public int ProjectStatusId { get;private set; }

    public string Name { get;private set; } = null!;

    public virtual ICollection<Project> Projects { get; private set; } = new List<Project>();

    private ProjectStatus() { }
    public ProjectStatus(string Name)
    {
        if (Name.Length > 20)
            throw new StringLengthException(nameof(Name));
        this.Name = Name;   
    }
}
