﻿using Havoc_API.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Havoc_API.Models
{
    public partial class Project
    {
        private string _name;
        private string? _description;
        private DateTime? _start;
        private DateTime? _deadline;
        private DateTime _lastModified;

        public int ProjectId { get; private set; }

        public string Name
        {
            get => _name;
            private set
            {
                if (value.Length > 25)
                    throw new StringLengthException(nameof(Name));
                _name = value;
            }
        }

        public string? Description
        {
            get => _description;
            private set
            {
                if (value != null && value.Length > 200)
                    throw new StringLengthException(nameof(Description));
                _description = value;
            }
        }

        public byte[]? Background { get; private set; }

        public int CreatorId { get; private set; }

        public DateTime? Start
        {
            get => _start;
            private set
            {
                if (value >= Deadline)
                    throw new WrongDateException("Start is after or equal to deadline");
                if (value < DateTime.Now)
                    throw new WrongDateException(nameof(Start) + ": " + value + "  Now: " + DateTime.Now);
                _start = value;
            }
        }

        public DateTime? Deadline
        {
            get => _deadline;
            private set
            {
                if (value <= Start)
                    throw new WrongDateException("Start is after or equal to deadline");
                if (value < DateTime.Now)
                    throw new WrongDateException(nameof(Deadline) + ": " + value + "  Now: " + DateTime.Now);
                _deadline = value;
            }
        }

        public DateTime LastModified 
        {
            get=>_lastModified;
            private set
            {
                if (value > DateTime.Now)
                    throw new WrongDateException("Last modified date is before current date.\n" +
                        "Last modified: "+value+"\n" +
                        "Now: "+DateTime.Now);
                _lastModified = value;
            }

        }

        public int ProjectStatusId { get; private set; }

        public virtual User Creator { get; private set; } = null!;

        public virtual ICollection<Participation> Participations { get; private set; } = new List<Participation>();

        public virtual ProjectStatus ProjectStatus { get; private set; } = null!;

        public virtual ICollection<Task> Tasks { get; private set; } = new List<Task>();

        private Project() { }

        public Project(string name, string? description, byte[]? background, DateTime? start, DateTime? deadline, User creator, ProjectStatus projectStatus)
        {
            Name = name;
            Description = description;
            Background = background;
            CreatorId = creator.UserId;
            Start = start;
            Deadline = deadline;
            LastModified = DateTime.Now;
            ProjectStatusId = projectStatus.ProjectStatusId;
            ProjectStatus = projectStatus;
            Creator = creator;
        }
    }
}
