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
    public class StudentService
    {
        private readonly StudentRepo _studentRepository;
        private readonly DepartmentRepo _departmentRepository;
        private readonly CourseRepo _courseRepository;
        private readonly IMapper _mapper;

        public StudentService(
            StudentRepo studentRepository,
            DepartmentRepo departmentRepository,
            CourseRepo courseRepository,
            IMapper mapper)
        {
            _studentRepository = studentRepository;
            _departmentRepository = departmentRepository;
            _courseRepository = courseRepository;
            _mapper = mapper;
        }

        public async Task<(IEnumerable<StudentSearchDto> Items, int TotalCount)> GetStudentsAsync(
            string? search, int? departmentId, int pageNumber, int pageSize)
        {
            if (departmentId.HasValue &&
                !await _departmentRepository.ExistsAsync(departmentId.Value))
            {
                throw new KeyNotFoundException("Department was not found.");
            }

            var result = await _studentRepository.GetAsync(
                search, departmentId, pageNumber, pageSize);

            return (
                _mapper.Map<IEnumerable<StudentSearchDto>>(result.Items),
                result.TotalCount
            );
        }

        public async Task<StudentDto?> GetStudentAsync(int id)
        {
            var student = await _studentRepository.GetByIdAsync(id);

            return student == null
                ? null
                : _mapper.Map<StudentDto>(student);
        }

        public async Task<StudentDto> CreateStudentAsync(CreateStudentDto dto)
        {
            if (!await _departmentRepository.ExistsAsync(dto.DepartmentId))
                throw new KeyNotFoundException("Department was not found.");

            if (await _studentRepository.EmailExistsAsync(dto.Email))
                throw new InvalidOperationException(
                    "A student with this email already exists.");

            var student = _mapper.Map<Student>(dto);

            await _studentRepository.AddAsync(student);
            await _studentRepository.SaveChangesAsync();

            var created = await _studentRepository.GetByIdAsync(student.StudentId);

            return _mapper.Map<StudentDto>(created);
        }

        public async Task<bool> UpdateStudentAsync(int id, UpdateStudentDto dto)
        {
            var student = await _studentRepository.GetByIdAsync(id);

            if (student == null)
                return false;

            if (!await _departmentRepository.ExistsAsync(dto.DepartmentId))
                throw new KeyNotFoundException("Department was not found.");

            if (await _studentRepository.EmailExistsAsync(dto.Email, id))
                throw new InvalidOperationException(
                    "A student with this email already exists.");

            _mapper.Map(dto, student);

            await _studentRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> PatchStudentAsync(int id, PatchStudentDto dto)
        {
            var student = await _studentRepository.GetByIdAsync(id);

            if (student == null)
                return false;

            if (dto.DepartmentId.HasValue &&
                !await _departmentRepository.ExistsAsync(dto.DepartmentId.Value))
            {
                throw new KeyNotFoundException("Department was not found.");
            }

            if (dto.Email != null &&
                await _studentRepository.EmailExistsAsync(dto.Email, id))
            {
                throw new InvalidOperationException(
                    "A student with this email already exists.");
            }

            if (dto.FirstName != null)
                student.FirstName = dto.FirstName;

            if (dto.LastName != null)
                student.LastName = dto.LastName;

            if (dto.Email != null)
                student.Email = dto.Email;

            if (dto.DepartmentId.HasValue)
                student.DepartmentId = dto.DepartmentId.Value;

            await _studentRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            var student = await _studentRepository.GetByIdAsync(id);

            if (student == null)
                return false;

            _studentRepository.Remove(student);
            await _studentRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EnrollStudentAsync(
            int studentId, CreateEnrollmentDto dto)
        {
            if (!await _studentRepository.ExistsAsync(studentId))
                throw new KeyNotFoundException("Student was not found.");

            if (!await _courseRepository.ExistsAsync(dto.CourseId))
                throw new KeyNotFoundException("Course was not found.");

            if (await _studentRepository.EnrollmentExistsAsync(
                studentId, dto.CourseId))
            {
                throw new InvalidOperationException(
                    "The student is already enrolled in this course.");
            }

            await _studentRepository.AddEnrollmentAsync(new Enrollment
            {
                StudentId = studentId,
                CourseId = dto.CourseId,
                EnrolledOn = DateTime.UtcNow
            });

            await _studentRepository.SaveChangesAsync();

            return true;
        }
    }

}