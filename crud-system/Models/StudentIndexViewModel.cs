using crud_system.Models;

public class StudentIndexViewModel
{
    public IEnumerable<Student> Students { get; set; }
    public int TotalStudents { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
