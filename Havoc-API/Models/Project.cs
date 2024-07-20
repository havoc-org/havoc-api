using Havoc_API.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Havoc_API.Models;

public partial class Project
{


    public int ProjectId { get; private set; }

    public string Name { get; private set; } = null!;

    public string? Description { get; private set; }

    public byte[]? Background { get; private set; }

    public int CreatorId { get; private set; }

    public DateTime? Start { get; private set; }

    public DateTime? Deadline { get; private set; }

    public DateTime LastModified { get; private set; }

    public int ProjectStatusId { get; private set; }

    public virtual User Creator { get; private set; } = null!;

    public virtual ICollection<Participation> Participations { get; private set; } = new List<Participation>();

    public virtual ProjectStatus ProjectStatus { get; private set; } = null!;

    public virtual ICollection<Task> Tasks { get; private set; } = new List<Task>();

    private Project() { }

    public Project(string Name,string? Description, byte[]? Background, DateTime? Start, DateTime? Deadline, User Creator, ProjectStatus ProjectStatus)
    {
        if (Name.Length > 25)
            throw new StringLengthException(nameof(Name));
        else if (Description != null && Description.Length>200)
            throw new StringLengthException(nameof(Description));
        else if(Start>Deadline)
            throw new WrongDateException("Start is after deadline");
        else if (Start < DateTime.Now)
            throw new WrongDateException(nameof(Start)+": "+Start);
        else if (Deadline < DateTime.Now)
            throw new WrongDateException(nameof(Deadline) + ": " + Deadline);

        this.Name = Name;
        this.Description = Description; 
        this.Background = Background;   
        this.CreatorId = Creator.UserId; 
        this.Start = Start; 
        this.Deadline = Deadline;
        this.LastModified = DateTime.Now;
        this.ProjectStatusId = ProjectStatus.ProjectStatusId;
        this.ProjectStatus = ProjectStatus;
        this.Creator=Creator;
    }
    /*public void addParticipations(ICollection<Participation> Participations)
    {
        foreach (var participation in Participations)
        {
            if (this.Participations.Contains(participation))
                throw new Exception("This participation already exists userID: "+participation.UserId+" projectID: "+participation.ProjectId);
            this.Participations.Add(participation);
            
        }
    }*/
}
