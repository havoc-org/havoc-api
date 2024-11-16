using Havoc_API.DTOs.Task;
using Havoc_API.DTOs.TaskStatus;

namespace Havoc_API.Services;

public interface ITaskService
{
    public Task<List<TaskGET>> GetTasksByProjectIdAsync(int projectId);
    public Task<int> AddTaskAsync(TaskPOST task);
    public Task<int> DeleteTaskByIdAsync(int taskId);
    public Task<int> UpdateTaskAsync(TaskPATCH taskUpdate);
    public Task<int> UpdateStatusByIdAsync(int taskId, TaskStatusPATCH taskStatus);
    public Task<Models.Task> GetTaskByIdAsync(int taskId);
    public Task<List<TaskStatusGET>> GetAllTaskStatusesAsync();
}