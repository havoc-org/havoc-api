using System;
using System.Collections.Generic;
using Havoc_API.Exceptions;

namespace Havoc_API.Models;

public partial class Comment
{
    private string _content = null!;
    private DateTime _commentDate;

    public int CommentId { get; private set; }

    public int TaskId { get; private set; }

    public int UserId { get; private set; }

    public string Content 
    { 
        get => _content;
        private set{
            string trimmedValue = value.Trim();

            if (trimmedValue.Length > 200 || trimmedValue.Length == 0)
                throw new StringLengthException(nameof(Content));
            
            _content = trimmedValue;
        } 
    }

    public DateTime CommentDate 
    { 
        get => _commentDate;
        private set
        {
            if (value > DateTime.Now)
                throw new WrongDateException("Comment Date is before current date.\n" +
                    "Comment Date: "+value+"\n" +
                    "Now: "+DateTime.Now);
            _commentDate = value;
        }
    }

    public virtual Task Task { get; private set; } = null!;

    public virtual User User { get; private set; } = null!;

    private Comment() { }

    public Comment(string content, DateTime commentDate, Task task, User user)
    {
        Content = content;
        CommentDate = commentDate;
        Task = task;
        TaskId = task.TaskId;
        User = user;
        UserId = user.UserId;
    }
}
