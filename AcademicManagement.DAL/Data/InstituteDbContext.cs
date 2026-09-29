using AcademicManagement.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicManagement.DAL.Data
{
    public class InstituteDbContext : DbContext
    {
        public InstituteDbContext(DbContextOptions<InstituteDbContext> options) : base(options)
        {
        }

        public DbSet<Institute> Institutes => Set<Institute>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<Instructor> Instructors => Set<Instructor>();
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<Enrollment> Enrollments => Set<Enrollment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Institute>()
                .HasKey(i => i.InstituteId);

            modelBuilder.Entity<Student>()
                .HasKey(s => s.StudentId);

            modelBuilder.Entity<Instructor>()
                .HasKey(i => i.InstructorId);

            modelBuilder.Entity<Course>()
                .HasKey(c => c.CourseId);

            modelBuilder.Entity<Enrollment>()
                .HasKey(e => new { e.StudentId, e.CourseId });

            modelBuilder.Entity<Institute>()
                .HasMany(i => i.Students)
                .WithOne(s => s.Institute)
                .HasForeignKey(s => s.InstituteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Instructor>()
                .HasMany(i => i.Courses)
                .WithOne(c => c.Instructor)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Student>()
                .HasMany(s => s.Enrollments)
                .WithOne(e => e.Student)
                .HasForeignKey(e => e.StudentId);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.Enrollments)
                .WithOne(e => e.Course)
                .HasForeignKey(e => e.CourseId);

            modelBuilder.Entity<Institute>().HasData(
                new Institute
                {
                    InstituteId = 1,
                    Name = "Northstar Institute",
                    City = "Winnipeg"
                },
                new Institute
                {
                    InstituteId = 2,
                    Name = "Riverside Institute",
                    City = "Brandon"
                }
            );

            modelBuilder.Entity<Instructor>().HasData(
                new Instructor
                {
                    InstructorId = 1,
                    FullName = "Sarah Johnson",
                    Email = "sarah@example.com"
                },
                new Instructor
                {
                    InstructorId = 2,
                    FullName = "David Brown",
                    Email = "david@example.com"
                }
            );

            modelBuilder.Entity<Course>().HasData(
                new Course
                {
                    CourseId = 1,
                    CourseCode = "WEB101",
                    CourseName = "Web Development",
                    InstructorId = 1
                },
                new Course
                {
                    CourseId = 2,
                    CourseCode = "DB101",
                    CourseName = "Database Fundamentals",
                    InstructorId = 2
                }
            );

            modelBuilder.Entity<Student>().HasData(
                new Student
                {
                    StudentId = 1,
                    FirstName = "John",
                    LastName = "Smith",
                    Email = "john@example.com",
                    InstituteId = 1
                },
                new Student
                {
                    StudentId = 2,
                    FirstName = "Mary",
                    LastName = "Brown",
                    Email = "mary@example.com",
                    InstituteId = 1
                },
                new Student
                {
                    StudentId = 3,
                    FirstName = "Alex",
                    LastName = "Wilson",
                    Email = "alex@example.com",
                    InstituteId = 2
                }
            );
        }
    }
}
