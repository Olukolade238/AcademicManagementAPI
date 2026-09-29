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
    public class DepartmentRepository
    {
        private readonly AcademicDbContext _context;
        public DepartmentRepository(AcademicDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            return await _context.Departments
                .AsNoTracking()
                .Include(d => d.Students)
                .OrderBy(d => d.Name)
                .ToListAsync();
        }
        public async Task<Department?> GetByIdAsync(int id)
        {
            return await _context.Departments
                .AsNoTracking()
                .Include(d => d.Students)
                .FirstOrDefaultAsync(d => d.DepartmentId == id);
        }
        public Task<bool> ExistsAsync(int id)
        {
            return _context.Departments.AnyAsync(d => d.DepartmentId == id);
        }
        public Task<bool> CodeExistsAsync(string code)
        {
            return _context.Departments.AnyAsync(d => d.Code == code);
        }
        public async Task AddAsync(Department department)
        {
            await _context.Departments.AddAsync(department);
        }
        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
