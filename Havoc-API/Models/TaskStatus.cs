using System;
using System.Collections.Generic;
using Havoc_API.Exceptions;

namespace Havoc_API.Models;

public partial class TaskStatus
{
    private string _name = null!;

    public int TaskStatusId { get; private set; }

    public string Name
    {
        get => _name;
        private set
        {
            string trimmedValue = value.Trim();

            if (trimmedValue.Length > 20 || trimmedValue.Length == 0)
                throw new StringLengthException(nameof(Name));

            _name = trimmedValue;
        }
    }

    public virtual ICollection<Task> Tasks { get; private set; } = new List<Task>();

    private TaskStatus() {  }

    public TaskStatus(string name)
    {
        Name = name;
    }
}
