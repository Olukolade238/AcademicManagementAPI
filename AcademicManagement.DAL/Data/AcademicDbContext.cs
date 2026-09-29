using AcademicManagement.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicManagement.DAL.Data
{
    public class AcademicDbContext : DbContext
    {
        public AcademicDbContext(DbContextOptions<AcademicDbContext> options) : base(options)
        {
        }

        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<Instructor> Instructors => Set<Instructor>();
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<Enrollment> Enrollments => Set<Enrollment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>().HasKey(d => d.DepartmentId);
            modelBuilder.Entity<Student>().HasKey(s => s.StudentId);
            modelBuilder.Entity<Instructor>().HasKey(i => i.InstructorId);
            modelBuilder.Entity<Course>().HasKey(c => c.CourseId);
            modelBuilder.Entity<Enrollment>().HasKey(e => new { e.StudentId, e.CourseId });

            // One-to-Many
            modelBuilder.Entity<Department>()
                .HasMany(d => d.Students)
                .WithOne(s => s.Department)
                .HasForeignKey(s => s.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Department>()
                .HasMany(d => d.Instructors)
                .WithOne(i => i.Department)
                .HasForeignKey(i => i.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Department>()
                .HasMany(d => d.Courses)
                .WithOne(c => c.Department)
                .HasForeignKey(c => c.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Instructor>()
                .HasMany(i => i.Courses)
                .WithOne(c => c.Instructor)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Many-to-Many
            modelBuilder.Entity<Student>()
                .HasMany(s => s.Enrollments)
                .WithOne(e => e.Student)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.Enrollments)
                .WithOne(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique values (duplicates will fail, which you can return as Conflict)
            modelBuilder.Entity<Student>().HasIndex(s => s.Email).IsUnique();
            modelBuilder.Entity<Instructor>().HasIndex(i => i.Email).IsUnique();
            modelBuilder.Entity<Course>().HasIndex(c => c.CourseCode).IsUnique();
            modelBuilder.Entity<Department>().HasIndex(d => d.Code).IsUnique();

            // Column type for grades
            modelBuilder.Entity<Enrollment>()
                .Property(e => e.Grade)
                .HasColumnType("decimal(5,2)");

            modelBuilder.Entity<Department>().HasData(
                new Department { DepartmentId = 1, Name = "Software Development 001", Code = "SD101" },
                new Department { DepartmentId = 2, Name = "Business Administration 001", Code = "BA101" }
            );

            modelBuilder.Entity<Instructor>().HasData(
                new Instructor { InstructorId = 1, FullName = "Sarah Johnson", Email = "sarah.johnson@college.ca", DepartmentId = 1 },
                new Instructor { InstructorId = 2, FullName = "Mary Claire", Email = "mary.claire@hitt.ca", DepartmentId = 1 },
                new Instructor { InstructorId = 3, FullName = "Pragya Patel", Email = "pragya.patel@college.ca", DepartmentId = 2 }
            );

            modelBuilder.Entity<Course>().HasData(
                new Course { CourseId = 1, CourseCode = "SD101", Title = "Object-Oriented Programming", Credits = 4, DepartmentId = 1, InstructorId = 1 },
                new Course { CourseId = 2, CourseCode = "SD201", Title = "Databases with SQL Server", Credits = 3, DepartmentId = 1, InstructorId = 2 },
                new Course { CourseId = 3, CourseCode = "BA101", Title = "Intro to Marketing", Credits = 3, DepartmentId = 2, InstructorId = 3 }
            );

            modelBuilder.Entity<Student>().HasData(
                new Student { StudentId = 1, FirstName = "Stephanie", LastName = "John", Email = "john.stephanie@student.ca", DepartmentId = 1 },
                new Student { StudentId = 2, FirstName = "Gold", LastName = "Brown", Email = "gold.brown@student.ca", DepartmentId = 1 },
                new Student { StudentId = 3, FirstName = "Alex", LastName = "Wilson", Email = "alex.wilson@student.ca", DepartmentId = 2 }
            );

            modelBuilder.Entity<Enrollment>().HasData(
                new Enrollment { StudentId = 1, CourseId = 1, EnrolledOn = new DateTime(2026, 9, 1), Grade = 88.5m },
                new Enrollment { StudentId = 1, CourseId = 2, EnrolledOn = new DateTime(2026, 9, 1), Grade = 92.0m },
                new Enrollment { StudentId = 2, CourseId = 1, EnrolledOn = new DateTime(2026, 9, 1) },
                new Enrollment { StudentId = 3, CourseId = 3, EnrolledOn = new DateTime(2026, 9, 1), Grade = 81.0m }
            );
        }
    }
}
