using System;
using System.Collections.Generic;

namespace Havoc_API.Models;

public partial class Role
{
    public int RoleId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Participation> Participations { get; set; } = new List<Participation>();
}
