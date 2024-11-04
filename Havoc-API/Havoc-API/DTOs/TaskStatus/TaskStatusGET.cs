using Havoc_API.DTOs.Task;

namespace Havoc_API.DTOs.TaskStatus;

public class TaskStatusGET
{
    public int TaskStatusId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<TaskGET> Tasks { get; set; } = new List<TaskGET>();

    public TaskStatusGET(int taskStatusId, string name)
    {
        TaskStatusId = taskStatusId;
        Name = name;
    }
}