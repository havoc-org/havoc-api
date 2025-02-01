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
}