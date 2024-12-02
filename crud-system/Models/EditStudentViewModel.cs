using System;
using System.ComponentModel.DataAnnotations;

namespace crud_system.Models
{
    public class EditStudentViewModel
    {
        [Required]
        public Guid StudentID { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string FirstName { get; set; }

        [StringLength(50, ErrorMessage = "Middle name cannot exceed 50 characters.")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        //[Phone(ErrorMessage = "Invalid phone number.")]
        public int PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Class type is required.")]
        [StringLength(50, ErrorMessage = "Class type cannot exceed 50 characters.")]
        public string ClassType { get; set; }

        [Required(ErrorMessage = "Course name is required.")]
        public string CourseName { get; set; }
    }
}
