using Havoc_API.Exceptions;
using System;
using System.Collections.Generic;

namespace Havoc_API.Models;

public partial class ProjectStatus
{
    private string _name = null!;

    public int ProjectStatusId { get; private set; }

    public string Name 
    {
        get => _name;
        private set
        {
            string trimmedValue = value.Trim();

            if (trimmedValue.Length > 20 || trimmedValue.Length == 0)
                throw new StringLengthException(nameof(Name));

            _name = trimmedValue;
        }
    }

    public virtual ICollection<Project> Projects { get; private set; } = new List<Project>();

    private ProjectStatus() { }
    
    public ProjectStatus(string name)
    {
        Name = name;   
    }
}
