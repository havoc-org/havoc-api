public class CommentPOST
{
    public string Content { get; set; } = null!;

    public CommentPOST(string content)
    {
        Content = content;
    }
}