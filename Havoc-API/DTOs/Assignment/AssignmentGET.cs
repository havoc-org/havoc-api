using Havoc_API.DTOs.Task;
using Havoc_API.DTOs.User;

namespace Havoc_API.DTOs.Assignment;

public class AssignmentGET
{
    public int UserId { get; set; }

    public int TaskId { get; set; }

    public string? Description { get; set; }

    public virtual TaskGET Task { get; set; } = null!;

    public virtual UserGET User { get; set; } = null!;

    public AssignmentGET(int userId, int taskId, string? description)
    {
        UserId = userId;
        TaskId = taskId;
        Description = description;
    }
}