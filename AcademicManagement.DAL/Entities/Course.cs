using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicManagement.DAL.Entities
{
    public class Course
    {
        public int CourseId { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public int InstructorId { get; set; }
        public Instructor Instructor { get; set; } = null!;
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
