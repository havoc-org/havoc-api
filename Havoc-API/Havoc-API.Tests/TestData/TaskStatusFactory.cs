using Havoc_API.DTOs.TaskStatus;

namespace Havoc_API.Tests.TestData;

public static class TaskStatusFactory
{
    public static TaskStatusPATCH CreatePatch(string name = "NewTaskStatus")
    {
        return new TaskStatusPATCH
        {
            Name = name
        };
    }

    public static TaskStatusPOST CreatePost(string name = "NewTaskStatus")
    {
        return new TaskStatusPOST
        {
            Name = name
        };
    }

    public static Models.TaskStatus Create(string name = "NewTaskStatus")
    {
        return new Models.TaskStatus(name);
    }
}