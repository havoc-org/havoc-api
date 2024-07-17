using Havoc_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havoc_API.Data
{
    public interface IHavocContext
    {

        DbSet<Assignment> Assignments { get; set; }

        DbSet<Attachment> Attachments { get; set; }

        DbSet<Comment> Comments { get; set; }

        DbSet<Participation> Participations { get; set; }

        DbSet<Project> Projects { get; set; }

        DbSet<ProjectStatus> ProjectStatuses { get; set; }

        DbSet<Role> Roles { get; set; }

        DbSet<Tag> Tags { get; set; }

        DbSet<Models.Task> Tasks { get; set; }

        DbSet<Models.TaskStatus> TaskStatuses { get; set; }

        DbSet<User> Users { get; set; }
        DatabaseFacade Database { get; }
        Task<int> SaveChangesAsync();
    }
}
