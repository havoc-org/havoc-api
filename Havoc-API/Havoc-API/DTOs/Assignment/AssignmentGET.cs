using Havoc_API.DTOs.Task;
using Havoc_API.DTOs.User;

namespace Havoc_API.DTOs.Assignment;

public class AssignmentGET
{
    public string? Description { get; set; }

    public virtual TaskGET Task { get; set; } = null!;

    public virtual UserGET User { get; set; } = null!;

    public AssignmentGET(UserGET user, string? description)
    {
        User = user;
        Description = description;
    }
}