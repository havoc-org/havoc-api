using Havoc_API.Exceptions;
using System;
using System.Collections.Generic;

namespace Havoc_API.Models;

public partial class ProjectStatus
{
    private string _name;

    public int ProjectStatusId { get;private set; }

    public string Name {
        get=>_name;
        private set 
        {  
            if(value.Length>20)
                throw new StringLengthException(nameof(Name));
            _name=value;
        }
    }

    public virtual ICollection<Project> Projects { get; private set; } = new List<Project>();

    private ProjectStatus() { }
    public ProjectStatus(string Name)
    {
        if (Name.Length > 20)
            throw new StringLengthException(nameof(Name));
        this.Name = Name;   
    }
}
