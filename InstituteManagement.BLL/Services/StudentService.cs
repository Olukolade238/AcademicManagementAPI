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
    public class StudentService
    {
        private readonly StudentRepo _studentRepo;
        private readonly DepartmentRepo _departmentRepo;
        private readonly CourseRepo _courseRepo;

        public StudentService(
            StudentRepo studentRepo,
            DepartmentRepo departmentRepo,
            CourseRepo courseRepo)
        {
            _studentRepo = studentRepo;
            _departmentRepo = departmentRepo;
            _courseRepo = courseRepo;
        }

        public async Task<(IEnumerable<StudentSearchDto> Items, int TotalCount)> GetStudentsAsync(
            string? search, int? departmentId, int pageNumber, int pageSize)
        {
            if (departmentId.HasValue &&
                !await _departmentRepo.ExistsAsync(departmentId.Value))
            {
                throw new KeyNotFoundException("Department was not found.");
            }

            var result = await _studentRepo.GetAsync(
                search, departmentId, pageNumber, pageSize);

            return (result.Items.ToSearchDtoList(), result.TotalCount);
        }

        public async Task<StudentDto?> GetStudentAsync(int id)
        {
            var student = await _studentRepo.GetByIdAsync(id);

            return student == null ? null : student.ToDto();
        }

        public async Task<StudentDto> CreateStudentAsync(CreateStudentDto dto)
        {
            if (!await _departmentRepo.ExistsAsync(dto.DepartmentId))
                throw new KeyNotFoundException("Department was not found.");

            if (await _studentRepo.EmailExistsAsync(dto.Email))
                throw new InvalidOperationException(
                    "A student with this email already exists.");

            var student = dto.ToEntity();

            await _studentRepo.AddAsync(student);
            await _studentRepo.SaveChangesAsync();

            var created = await _studentRepo.GetByIdAsync(student.StudentId);

            return created!.ToDto();
        }

        public async Task<bool> UpdateStudentAsync(int id, UpdateStudentDto dto)
        {
            var student = await _studentRepo.GetByIdAsync(id);

            if (student == null)
                return false;

            if (!await _departmentRepo.ExistsAsync(dto.DepartmentId))
                throw new KeyNotFoundException("Department was not found.");

            if (await _studentRepo.EmailExistsAsync(dto.Email, id))
                throw new InvalidOperationException(
                    "A student with this email already exists.");

            student.UpdateFrom(dto);

            await _studentRepo.SaveChangesAsync();

            return true;
        }

        public async Task<bool> PatchStudentAsync(int id, PatchStudentDto dto)
        {
            var student = await _studentRepo.GetByIdAsync(id);

            if (student == null)
                return false;

            if (dto.DepartmentId.HasValue &&
                !await _departmentRepo.ExistsAsync(dto.DepartmentId.Value))
            {
                throw new KeyNotFoundException("Department was not found.");
            }

            if (dto.Email != null &&
                await _studentRepo.EmailExistsAsync(dto.Email, id))
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

            await _studentRepo.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            var student = await _studentRepo.GetByIdAsync(id);

            if (student == null)
                return false;

            _studentRepo.Remove(student);
            await _studentRepo.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EnrollStudentAsync(
            int studentId, CreateEnrollmentDto dto)
        {
            if (!await _studentRepo.ExistsAsync(studentId))
                throw new KeyNotFoundException("Student was not found.");

            if (!await _courseRepo.ExistsAsync(dto.CourseId))
                throw new KeyNotFoundException("Course was not found.");

            if (await _studentRepo.EnrollmentExistsAsync(
                studentId, dto.CourseId))
            {
                throw new InvalidOperationException(
                    "The student is already enrolled in this course.");
            }

            await _studentRepo.AddEnrollmentAsync(new Enrollment
            {
                StudentId = studentId,
                CourseId = dto.CourseId,
                EnrolledOn = DateTime.UtcNow
            });

            await _studentRepo.SaveChangesAsync();

            return true;
        }
    }

}