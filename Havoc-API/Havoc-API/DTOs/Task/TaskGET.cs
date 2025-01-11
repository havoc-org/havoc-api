using Havoc_API.DTOs.Project;
using Havoc_API.DTOs.User;
using Havoc_API.DTOs.TaskStatus;
using Havoc_API.DTOs.Tag;
using Havoc_API.DTOs.Comment;
using Havoc_API.DTOs.Attachment;
using Havoc_API.DTOs.Assignment;

namespace Havoc_API.DTOs.Task;

public class TaskGET
{
    public int TaskId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? Start { get; set; }

    public DateTime? Deadline { get; set; }

    public int ProjectId {  get; set; }

    public virtual ICollection<AssignmentGET> Assignments { get; set; } = new List<AssignmentGET>();

    public virtual ICollection<AttachmentGET> Attachments { get; set; } = new List<AttachmentGET>();

    public virtual ICollection<CommentGET> Comments { get; set; } = new List<CommentGET>();

    public virtual ICollection<TagGET> Tags { get; set; } = new List<TagGET>();

    public virtual UserGET Creator { get; set; } = null!;

    public virtual TaskStatusGET TaskStatus { get; set; } = null!;
    

    private TaskGET(){}

    public TaskGET(int taskId, string name, string? description, DateTime? start, DateTime? deadline,int projectId, UserGET creator, TaskStatusGET taskStatus, ICollection<AssignmentGET> assignments, ICollection<AttachmentGET> attachments, ICollection<CommentGET> comments, ICollection<TagGET> tags){
        TaskId = taskId;
        Name = name;
        Description = description;
        Start = start;
        Deadline = deadline;
        ProjectId=projectId;
        Creator = creator;
        TaskStatus = taskStatus;
        Assignments = assignments;
        Attachments = attachments;
        Comments = comments;
        Tags = tags;
    }
}