using AcademicManagement.BLL.DTOs;
using AcademicManagement.DAL.Entities;
using AcademicManagement.DAL.Repository;
using AcademicManagement.BLL.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicManagement.BLL.Services
{
    public class DepartmentService
    {
        private readonly DepartmentRepo _repository;

        public DepartmentService(DepartmentRepo repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<DepartmentDto>> GetDepartmentsAsync()
        {
            var departments = await _repository.GetAllAsync();
            return departments.ToDtoList();
        }

        public async Task<DepartmentDto?> GetDepartmentAsync(int id)
        {
            var department = await _repository.GetByIdAsync(id);
            return department == null ? null : department.ToDto();
        }

        public async Task<DepartmentDto> CreateDepartmentAsync(CreateDepartmentDto dto)
        {
            if (await _repository.CodeExistsAsync(dto.Code))
                throw new InvalidOperationException("A department with this code already exists.");

            var department = dto.ToEntity();

            await _repository.AddAsync(department);
            await _repository.SaveChangesAsync();

            return department.ToDto();
        }
    }
}
