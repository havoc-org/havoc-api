using Havoc_API.DTOs.Assignment;
using Havoc_API.Models;

public static class AssignmentFactory
{
    public static Assignment Create(User user, Havoc_API.Models.Task task)
    {
        return new Assignment(
            "Test assignment",
            task,
            user
        );
    }

    public static AssignmentDELETE CreateDelete(int userId)
    {
        return new AssignmentDELETE
        {
            UserId = userId
        };
    }

    public static AssignmentPOST CreatePost(int userId, string? description = "Test description")
    {
        return new AssignmentPOST
        {
            UserId = userId,
            Description = description
        };
    }
}