using AcademicManagement.DAL.Data;
using AcademicManagement.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicManagement.DAL.Repository
{
    public class StudentRepo
    {
        private readonly AcademicDbContext _context;
        public StudentRepo(AcademicDbContext context)
        {
            _context = context;
        }
        // Finds students based on selected filters and returns the requested page of results plus the total number found.
        public async Task<(IEnumerable<Student> Items, int TotalCount)> GetAsync(
        string? search, int? departmentId, int pageNumber, int pageSize)
        {
            IQueryable<Student> query = _context.Students
                .AsNoTracking()
                .Include(s => s.Department);
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(s =>
                    s.FirstName.Contains(search) ||
                    s.LastName.Contains(search) ||
                    s.Email.Contains(search));
            }
            if (departmentId.HasValue)
            {
                query = query.Where(s => s.DepartmentId == departmentId.Value);
            }
            var totalCount = await query.CountAsync();
            var students = await query
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (students, totalCount);
        }
        public async Task<Student?> GetByIdAsync(int id)
        {
            return await _context.Students
                .Include(s => s.Department)
                .FirstOrDefaultAsync(s => s.StudentId == id);
        }
        public Task<bool> ExistsAsync(int id)
        {
            return _context.Students.AnyAsync(s => s.StudentId == id);
        }
        public Task<bool> EmailExistsAsync(string email, int? excludingStudentId = null)
        {
            var query = _context.Students.Where(s => s.Email == email);

            if (excludingStudentId.HasValue)
            {
                query = query.Where(s => s.StudentId != excludingStudentId.Value);
            }

            return query.AnyAsync();
        }
        public async Task AddAsync(Student student)
        {
            await _context.Students.AddAsync(student);
        }
        public void Remove(Student student)
        {
            _context.Students.Remove(student);
        }
        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Enrollment>?> GetEnrollmentsAsync(int studentId)
        {
            var studentExists = await ExistsAsync(studentId);

            if (!studentExists)
                return null;

            return await _context.Enrollments
                .AsNoTracking()
                .Include(e => e.Course)
                    .ThenInclude(c => c.Instructor)
                .Where(e => e.StudentId == studentId)
                .OrderBy(e => e.Course.Title)
                .ToListAsync();
        }
        public Task<bool> EnrollmentExistsAsync(int studentId, int courseId)
        {
            return _context.Enrollments.AnyAsync(
                e => e.StudentId == studentId && e.CourseId == courseId);
        }
        public async Task AddEnrollmentAsync(Enrollment enrollment)
        {
            await _context.Enrollments.AddAsync(enrollment);
        }
        public Task<Enrollment?> GetEnrollmentAsync(int studentId, int courseId)
        {
            return _context.Enrollments.FirstOrDefaultAsync(
                e => e.StudentId == studentId && e.CourseId == courseId);
        }
        public void RemoveEnrollment(Enrollment enrollment)
        {
            _context.Enrollments.Remove(enrollment);
        }
    }
}
