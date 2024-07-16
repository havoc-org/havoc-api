using System;
using System.Collections.Generic;

namespace Havoc_API.Models;

public partial class Participation
{
    public int ProjectId { get; set; }

    public int UserId { get; set; }

    public int RoleId { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
