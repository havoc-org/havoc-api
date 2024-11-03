using System;
using System.Collections.Generic;
using Havoc_API.Exceptions;

namespace Havoc_API.Models;

public partial class Attachment
{
    private string _fileLink = null!;

    public int AttachmentId { get; private set; }

    public int UserId { get; private set; }

    public int TaskId { get; private set; }

    public string FileLink 
    {
        get => _fileLink;
        private set
        {
            string trimmedValue = value.Trim();

            if (trimmedValue.Length > 255 || trimmedValue.Length == 0)
                throw new StringLengthException(nameof(FileLink));

            _fileLink = trimmedValue;
        } 
    }

    public virtual Task Task { get; private set; } = null!;

    public virtual User User { get; private set; } = null!;

    private Attachment() {  }

    public Attachment(string fileLink, Task task, User user)
    {
        FileLink = fileLink;
        Task = task;
        TaskId = task.TaskId;
        User = user;
        UserId = user.UserId;
    }
}
