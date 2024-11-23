using Havoc_API.DTOs.TaskStatus;

namespace Havoc_API.Tests.TestData;

public static class TaskStatusFactory
{
    public static TaskStatusPATCH CreatePatch()
    {
        return new TaskStatusPATCH
        {
            Name = "NewTaskStatus"
        };
    }

    public static Models.TaskStatus Create()
    {
        return new Models.TaskStatus("NewTaskStatus");
    }
}