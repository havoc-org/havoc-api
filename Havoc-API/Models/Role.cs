using System;
using System.Collections.Generic;
using Havoc_API.Exceptions;

namespace Havoc_API.Models;

public partial class Role
{
    private string _name = null!;

    public int RoleId { get; private set; }

    public string Name 
    {
        get => _name;
        private set
        {
            string trimmedValue = value.Trim();

            if (trimmedValue.Length > 25 || trimmedValue.Length == 0)
                throw new StringLengthException(nameof(Name));

            _name = trimmedValue;
        } 
    }

    public virtual ICollection<Participation> Participations { get; set; } = new List<Participation>();

    private Role() { }

    public Role(string name)
    {
        Name = name;
    }
}
