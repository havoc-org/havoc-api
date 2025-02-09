using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace Havoc_API.Models;

public partial class Participation
{
    public int ProjectId { get; private set; }

    public int UserId { get; private set; }

    public int RoleId { get; private set; }

    public virtual Project Project { get; private set; } = null!;

    public virtual Role Role { get; private set; } = null!;

    public virtual User User { get; private set; } = null!;

    private Participation() { }

    public Participation(Project project, Role role, User user)
    {
        ProjectId = project.ProjectId;
        UserId = user.UserId;
        RoleId = role.RoleId;
        Project = project;
        Role = role;
        User = user;
    }

    public void updateParticipationRole(Role role){
        RoleId = role.RoleId;
    }
}
