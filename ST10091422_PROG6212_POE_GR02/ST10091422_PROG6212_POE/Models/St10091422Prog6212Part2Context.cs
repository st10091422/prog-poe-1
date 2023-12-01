using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ST10091422_PROG6212_POE.Models;

public partial class St10091422Prog6212Part2Context : DbContext
{
    public St10091422Prog6212Part2Context()
    {
    }

    public St10091422Prog6212Part2Context(DbContextOptions<St10091422Prog6212Part2Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Module> Modules { get; set; }

    public virtual DbSet<Semester> Semesters { get; set; }

    public virtual DbSet<User> Users { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
       // => optionsBuilder.UseSqlServer("Data Source=DESKTOP-55EL6RG;Initial Catalog=ST10091422_PROG6212_PART2;Encrypt=False;Integrated Security=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Module>(entity =>
        {
            entity.HasKey(e => e.ModuleId).HasName("Module_pk");

            entity.ToTable("Module");

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CurrentDate).HasColumnType("date");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RemainingHours).HasColumnName("remainingHours");

            entity.HasOne(d => d.Semester).WithMany(p => p.Modules)
                .HasForeignKey(d => d.SemesterId)
                .HasConstraintName("Semester_Module_fk");
        });

        modelBuilder.Entity<Semester>(entity =>
        {
            entity.HasKey(e => e.SemesterId).HasName("Semester_pk");

            entity.ToTable("Semester");

            entity.Property(e => e.CurrentDate)
                .HasColumnType("date")
                .HasColumnName("currentDate");
            entity.Property(e => e.StartDate)
                .HasColumnType("date")
                .HasColumnName("startDate");
            entity.Property(e => e.WeekStart)
                .HasColumnType("date")
                .HasColumnName("weekStart");

            entity.HasOne(d => d.User).WithMany(p => p.Semesters)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("User_Semester_FK");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("User_pk");

            entity.ToTable("User");

            entity.Property(e => e.Password).HasMaxLength(32);
            entity.Property(e => e.Salt).HasMaxLength(16);
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.tempPass)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
