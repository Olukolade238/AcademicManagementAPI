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
    public class CourseRepository
    {
        private readonly AcademicDbContext _context;
        public CourseRepository(AcademicDbContext context)
        {

            _context = context;
        }
        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            return await _context.Courses
                .AsNoTracking()
                .Include(c => c.Department)
                .Include(c => c.Instructor)
                .OrderBy(c => c.CourseCode)
                .ToListAsync();
        }
        public Task<bool> ExistsAsync(int id)
        {
            return _context.Courses.AnyAsync(c => c.CourseId == id);
        }
    }
}
