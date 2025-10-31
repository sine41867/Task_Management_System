using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TMS_DAL.Entities;

public partial class DbTaskManagementSystemContext : DbContext
{
    public DbTaskManagementSystemContext()
    {
    }

    public DbTaskManagementSystemContext(DbContextOptions<DbTaskManagementSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<TaskHasTaskStatus> TaskHasTaskStatuses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=DB_TaskManagementSystem;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Task>(entity =>
        {
            entity.ToTable("Task", "tsk");

            entity.HasIndex(e => e.EntryIdentifier, "IX_Task_Identifier").IsUnique();

            entity.Property(e => e.AssignedTo).HasMaxLength(128);
            entity.Property(e => e.CreatedBy).HasMaxLength(128);
            entity.Property(e => e.DeletedBy).HasMaxLength(128);
            entity.Property(e => e.EntryIdentifier).HasMaxLength(128);
            entity.Property(e => e.Title).HasMaxLength(2000);
            entity.Property(e => e.UpdatedBy).HasMaxLength(128);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TaskCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Task_User");

            entity.HasOne(d => d.DeletedByNavigation).WithMany(p => p.TaskDeletedByNavigations)
                .HasForeignKey(d => d.DeletedBy)
                .HasConstraintName("FK_Task_User2");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.TaskUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_Task_User1");
        });

        modelBuilder.Entity<TaskHasTaskStatus>(entity =>
        {
            entity.ToTable("TaskHasTaskStatus", "tsk");

            entity.Property(e => e.CreatedBy).HasMaxLength(128);
            entity.Property(e => e.DeletedBy).HasMaxLength(128);
            entity.Property(e => e.UpdatedBy).HasMaxLength(128);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TaskHasTaskStatusCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskHasTaskStatus_User");

            entity.HasOne(d => d.DeletedByNavigation).WithMany(p => p.TaskHasTaskStatusDeletedByNavigations)
                .HasForeignKey(d => d.DeletedBy)
                .HasConstraintName("FK_TaskHasTaskStatus_User2");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskHasTaskStatuses)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskHasTaskStatus_Task");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.TaskHasTaskStatusUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_TaskHasTaskStatus_User1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.HasIndex(e => e.Username, "IX_Username").IsUnique();

            entity.Property(e => e.UserId).HasMaxLength(128);
            entity.Property(e => e.CreatedBy).HasMaxLength(128);
            entity.Property(e => e.DeletedBy).HasMaxLength(128);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(128);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
