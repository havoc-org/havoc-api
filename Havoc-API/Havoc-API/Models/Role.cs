using System.Text.Json.Serialization;


namespace Havoc_API.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RoleType
    {
        Owner,
        Manager,
        Developer
    }
    
public partial class Role
{
    public int RoleId { get; private set; }

    public RoleType Name {get; private set;}

    public virtual ICollection<Participation> Participations { get; set; } = new List<Participation>();

    private Role() { }

    public Role(RoleType name)
    {
        Name = name;
    }

    public bool CanCreateTask()
    {
        return Name == RoleType.Owner || Name == RoleType.Manager;
    }

    public bool CanDeleteProject()
    {
        return Name == RoleType.Owner;
    }

    public bool CanDeleteTask()
    {
        return Name == RoleType.Owner || Name == RoleType.Manager;
    }

    public bool CanEditProject()
    {
        return Name == RoleType.Owner;
    }

    public bool CanEditTask()
    {
        return Name == RoleType.Owner || Name == RoleType.Manager;
    }
}
