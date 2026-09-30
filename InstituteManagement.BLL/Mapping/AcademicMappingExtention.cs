using AcademicManagement.BLL.DTOs;
using AcademicManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicManagement.BLL.Mapping
{
    public static class AcademicMappingExtention
    {
        public static StudentDto ToDto(this Student student)
        {
            return new StudentDto
            {
                StudentId = student.StudentId,
                FullName = student.FirstName + " " + student.LastName,
                Email = student.Email,
                DepartmentId = student.DepartmentId,
                DepartmentName = student.Department.Name
            };
        }

        public static StudentSearchDto ToSearchDto(this Student student)
        {
            return new StudentSearchDto
            {
                StudentId = student.StudentId,
                FullName = student.FirstName + " " + student.LastName,
                Email = student.Email,
                DepartmentName = student.Department.Name
            };
        }

        public static Student ToEntity(this CreateStudentDto dto)
        {
            return new Student
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                DepartmentId = dto.DepartmentId
            };
        }

        public static void UpdateFrom(this Student student, UpdateStudentDto dto)
        {
            student.FirstName = dto.FirstName;
            student.LastName = dto.LastName;
            student.Email = dto.Email;
            student.DepartmentId = dto.DepartmentId;
        }

        // Department 
        public static DepartmentDto ToDto(this Department department)
        {
            return new DepartmentDto
            {
                DepartmentId = department.DepartmentId,
                Name = department.Name,
                Code = department.Code,
                StudentCount = department.Students.Count
            };
        }

        public static Department ToEntity(this CreateDepartmentDto dto)
        {
            return new Department { Name = dto.Name, Code = dto.Code };
        }

        // Course
        public static CourseDto ToDto(this Course course)
        {
            return new CourseDto
            {
                CourseId = course.CourseId,
                CourseCode = course.CourseCode,
                Title = course.Title,
                Credits = course.Credits,
                DepartmentId = course.DepartmentId,
                DepartmentName = course.Department.Name,
                InstructorId = course.InstructorId,
                InstructorName = course.Instructor.FullName
            };
        }

        // Enrollment 
        public static EnrollmentDto ToDto(this Enrollment enrollment)
        {
            return new EnrollmentDto
            {
                StudentId = enrollment.StudentId,
                CourseId = enrollment.CourseId,
                CourseCode = enrollment.Course.CourseCode,
                CourseTitle = enrollment.Course.Title,
                InstructorName = enrollment.Course.Instructor.FullName,
                EnrolledOn = enrollment.EnrolledOn,
                Grade = enrollment.Grade
            };
        }

        public static IEnumerable<StudentDto> ToDtoList(this IEnumerable<Student> students) =>
            students.Select(s => s.ToDto());

        public static IEnumerable<StudentSearchDto> ToSearchDtoList(this IEnumerable<Student> students) =>
            students.Select(s => s.ToSearchDto());

        public static IEnumerable<DepartmentDto> ToDtoList(this IEnumerable<Department> departments) =>
            departments.Select(d => d.ToDto());

        public static IEnumerable<CourseDto> ToDtoList(this IEnumerable<Course> courses) =>
            courses.Select(c => c.ToDto());
    }
}
