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
    public class StudentRepository
    {
        private readonly AcademicDbContext _context;
        public StudentRepository(AcademicDbContext context)
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
    }
}
