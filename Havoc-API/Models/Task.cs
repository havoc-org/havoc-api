using System;
using System.Collections.Generic;
using Havoc_API.Exceptions;

namespace Havoc_API.Models;

public partial class Task
{
    private string _name;
    private string? _description;
    private DateTime? _start;
    private DateTime? _deadline;
    public int TaskId { get; set; }

    public int ProjectId { get; set; }

    public string Name
    {
        get => _name;
        private set
        {
            string trimmedValue = value.Trim();

            if (trimmedValue.Length > 25 || trimmedValue.Length == 0)
                throw new StringLengthException(nameof(Name));

            _name = trimmedValue;
        }
    }

    public string? Description
    {
        get => _description;
        private set
        {
            if (value != null)
            {
                string trimmedValue = value.Trim();

                if (trimmedValue.Length > 200 || trimmedValue.Length == 0)
                    throw new StringLengthException(nameof(Description));

                _description = trimmedValue;
            }
            _description = value;
        }
    }

    public DateTime? Start
    {
        get => _start;
        private set
        {
            if (value.HasValue && Deadline.HasValue && value >= Deadline)
                throw new WrongDateException("Start is after or equal to deadline");
            
            _start = value;
        }
    }

    public DateTime? Deadline
    {
        get => _deadline;
        private set
        {
            if (value < DateTime.Now)
                throw new WrongDateException(nameof(Deadline) + ": " + value + "  Now: " + DateTime.Now);

            _deadline = value;
        }
    }

    public int CreatorId { get; set; }

    public int TaskStatusId { get; set; }

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual User Creator { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;

    public virtual TaskStatus TaskStatus { get; set; } = null!;

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();


    private Task(){}

    public Task(string name, string? description, DateTime? start, DateTime? deadline, User creator, Project project, TaskStatus taskStatus){
        Name = name;
        Description = description;
        Start = start;
        Deadline = deadline;
        Creator = creator;
        Project = project;
        TaskStatus = taskStatus;
    }
}
