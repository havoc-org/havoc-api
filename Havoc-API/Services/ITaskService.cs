using Havoc_API.DTOs.Task;

namespace Havoc_API.Services;

public interface ITaskService
{
    public Task<List<TaskGET>> GetTasksByProjectIdAsync(int projectId);

    public Task<int> AddTaskAsync(TaskPOST task);
}