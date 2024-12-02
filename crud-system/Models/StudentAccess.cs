namespace crud_system.Models
{
    public class StudentAccess
    {
        public Guid AccessNumber { get; set; }

        public Guid StudentID { get; set; } // Foreign key to Student

        public string Username { get; set; }

        public string Password { get; set; }

        public string CourseName { get; set; }

        public string CourseType { get; set; }

        // Navigation property for the related student
        public Student Student { get; set; }
    }

}
