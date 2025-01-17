using Havoc_API.DTOs.Task;
using Havoc_API.DTOs.TaskStatus;

namespace Havoc_API.Services;

public interface ITaskService
{
    public Task<List<TaskGET>> GetTasksByProjectIdAsync(int projectId);
    public Task<int> AddTaskAsync(TaskPOST task);
    public Task<int> DeleteTaskByIdAsync(int taskId);
    public Task<int> UpdateTaskAsync(TaskPATCH taskUpdate);
    public Task<int> UpdateTaskStatusAsync(TaskStatusPATCH taskStatus);
    public Task<TaskGET> GetTaskByIdAsync(int taskId);
    public Task<List<TaskStatusGET>> GetAllTaskStatusesAsync();
    public Task<List<TaskGET>> GetTasksAsync();
}