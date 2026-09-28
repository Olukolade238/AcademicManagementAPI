using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicManagement.DAL.Entities
{
    public class Student
    {
        public int StudentId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int InstituteId { get; set; }
        public Institute Institute { get; set; } = null!;
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
