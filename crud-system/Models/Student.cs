using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace crud_system.Models
{
    public class Student
    {
        [Key]
        public Guid StudentID { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public int PhoneNumber { get; set; }

        public string Email { get; set; }

        public string ClassType { get; set; }

        public string CourseName { get; set; } // Foreign key to Course


        // Navigation property for the related course
        public Course Course { get; set; }

        // Navigation property for related student access
        public StudentAccess StudentAccess { get; set; }
    }

}
