public class CommentPOST
{
    public string Content { get; set; } = null!;
    public int taskId { get; set; }
    public int projectId { get; set; }

    public CommentPOST(string content)
    {
        Content = content;
    }
}