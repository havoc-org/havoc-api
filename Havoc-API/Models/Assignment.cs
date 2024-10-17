using System;
using System.Collections.Generic;
using Havoc_API.Exceptions;

namespace Havoc_API.Models;

public partial class Assignment
{
    private string? _description;

    public int UserId { get; private set; }

    public int TaskId { get; private set; }

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

    public virtual Task Task { get; private set; } = null!;

    public virtual User User { get; private set; } = null!;

    private Assignment() {  }

    public Assignment(string? description, Task task, User user)
    {
        Description = description;
        Task = task;
        TaskId = task.TaskId;
        User = user;
        UserId = user.UserId;
    }
}
