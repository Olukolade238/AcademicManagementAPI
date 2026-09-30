using AcademicManagement.BLL.DTOs;
using AcademicManagement.DAL.Entities;
using AcademicManagement.DAL.Repository;
using AutoMapper;
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
        private readonly IMapper _mapper;
        public DepartmentService(
            DepartmentRepo repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<DepartmentDto>> GetDepartmentsAsync()
        {
            IEnumerable<Department> departments =
                await _repository.GetAllAsync();

            return _mapper.Map<IEnumerable<DepartmentDto>>(departments);
        }
        public async Task<DepartmentDto?> GetDepartmentAsync(int id)
        {
            Department? department =
                await _repository.GetByIdAsync(id);
            if (department == null)
            {
                return null;
            }
            return _mapper.Map<DepartmentDto>(department);
        }
        public async Task<DepartmentDto> CreateDepartmentAsync(
            CreateDepartmentDto dto)
        {
            if (await _repository.CodeExistsAsync(dto.Code))
            {
                throw new InvalidOperationException(
                    "A department with this code already exists.");
            }
            Department department =
                _mapper.Map<Department>(dto);
            await _repository.AddAsync(department);
            await _repository.SaveChangesAsync();
            return _mapper.Map<DepartmentDto>(department);
        }
    }
}
