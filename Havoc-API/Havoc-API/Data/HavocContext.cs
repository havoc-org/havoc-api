using System;
using System.Collections.Generic;
using Havoc_API.Models;
using Microsoft.EntityFrameworkCore;


namespace Havoc_API.Data;

public partial class HavocContext : DbContext, IHavocContext
{
    public HavocContext()
    {
    }

    public HavocContext(DbContextOptions<HavocContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Assignment> Assignments { get; set; }

    public virtual DbSet<Attachment> Attachments { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Participation> Participations { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectStatus> ProjectStatuses { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<Models.Task> Tasks { get; set; }

    public virtual DbSet<Models.TaskStatus> TaskStatuses { get; set; }

    public virtual DbSet<User> Users { get; set; }

   
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.TaskId }).HasName("Assignment_pk");

            entity.ToTable("Assignment");

            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.Task).WithMany(p => p.Assignments)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Assignment_Task");

            entity.HasOne(d => d.User).WithMany(p => p.Assignments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Assingment_User");
        });

        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.HasKey(e => e.AttachmentId).HasName("Attachment_pk");

            entity.ToTable("Attachment");

            entity.Property(e => e.FileLink)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Task).WithMany(p => p.Attachments)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Attachment_Task");

            entity.HasOne(d => d.User).WithMany(p => p.Attachments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Attachment_User");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("Comment_pk");

            entity.ToTable("Comment");

            entity.Property(e => e.CommentDate).HasColumnType("datetime");
            entity.Property(e => e.Content)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.Task).WithMany(p => p.Comments)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Comment_Task");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Comment_User");
        });

        modelBuilder.Entity<Participation>(entity =>
        {
            entity.HasKey(e => new { e.ProjectId, e.UserId }).HasName("Participation_pk");

            entity.ToTable("Participation");

            entity.HasOne(d => d.Project).WithMany(p => p.Participations)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Participation_Project");

            entity.HasOne(d => d.Role).WithMany(p => p.Participations)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Participation_Role");

            entity.HasOne(d => d.User).WithMany(p => p.Participations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Participation_User");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("Project_pk");

            entity.ToTable("Project");

            entity.Property(e => e.Deadline).HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.LastModified).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(25)
                .IsUnicode(false);
            entity.Property(e => e.Start).HasColumnType("datetime");

            entity.HasOne(d => d.Creator).WithMany(p => p.Projects)
                .HasForeignKey(d => d.CreatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Project_User");

            entity.HasOne(d => d.ProjectStatus).WithMany(p => p.Projects)
                .HasForeignKey(d => d.ProjectStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Project_ProjectStatus");
        });

        modelBuilder.Entity<ProjectStatus>(entity =>
        {
            entity.HasKey(e => e.ProjectStatusId).HasName("ProjectStatus_pk");

            entity.ToTable("ProjectStatus");

            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("Role_pk");

            entity.ToTable("Role");

            entity.Property(e => e.Name)
                .HasConversion<string>()
                .HasMaxLength(25)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.TagId).HasName("Tag_pk");

            entity.ToTable("Tag");

            entity.Property(e => e.ColorHex)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasColumnName("ColorHEX");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasMany(d => d.Tasks).WithMany(p => p.Tags)
                .UsingEntity<Dictionary<string, object>>(
                    "TaskTag",
                    r => r.HasOne<Models.Task>().WithMany()
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("Type_Task"),
                    l => l.HasOne<Tag>().WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("Type_Tag"),
                    j =>
                    {
                        j.HasKey("TagId", "TaskId").HasName("TaskTag_pk");
                        j.ToTable("TaskTag");
                    });
        });

        modelBuilder.Entity<Models.Task>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("Task_pk");

            entity.ToTable("Task");

            entity.Property(e => e.Deadline).HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(25)
                .IsUnicode(false);
            entity.Property(e => e.Start).HasColumnType("datetime");

            entity.HasOne(d => d.Creator).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.CreatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Task_User");

            entity.HasOne(d => d.Project).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Task_Project");

            entity.HasOne(d => d.TaskStatus).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.TaskStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Task_TaskStatus");
        });

        modelBuilder.Entity<Models.TaskStatus>(entity =>
        {
            entity.HasKey(e => e.TaskStatusId).HasName("TaskStatus_pk");

            entity.ToTable("TaskStatus");

            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("User_pk");

            entity.ToTable("User");

            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(128)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }
    public async Task<int> SaveChangesAsync()
    {
        return await base.SaveChangesAsync();
    }
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
