using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using crud_system.Models;
using crud_system.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

public class StudentsController : Controller
{
    private readonly CourseManagementContext _context;

    public StudentsController(CourseManagementContext context)
    {
        _context = context;
    }

    // GET: Students
    public async Task<IActionResult> Index()
    {
        var students = _context.Students.Include(s => s.Course);
        return View(await students.ToListAsync());
    }

    // GET: Students/Create
    public IActionResult Create()
    {
        ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "CourseName");
        return View();
    }

    // POST: Students/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("FirstName,MiddleName,LastName,PhoneNumber,Email,ClassType,CourseID")] Student student)
    {
        if (ModelState.IsValid)
        {
            student.StudentID = Guid.NewGuid(); // Generate unique ID for the student
            _context.Add(student);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Student added successfully!";
            return RedirectToAction(nameof(Index));
        }
        ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "CourseName", student.CourseID);
        return View(student);
    }

    // GET: Students/Edit/5
    public async Task<IActionResult> Edit(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest("Invalid student ID.");
        }

        var student = await _context.Students.FindAsync(id);
        if (student == null)
        {
            return NotFound("Student not found.");
        }

        ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "CourseName", student.CourseID);
        return View(student);
    }

    // POST: Students/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, [Bind("StudentID,FirstName,MiddleName,LastName,PhoneNumber,Email,ClassType,CourseID")] Student student)
    {
        if (id != student.StudentID)
        {
            return BadRequest("Student ID mismatch.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Entry(student).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Student updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await StudentExists(student.StudentID))
                {
                    return NotFound("Student no longer exists.");
                }
                throw;
            }
        }

        ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "CourseName", student.CourseID);
        return View(student);
    }

    // GET: Students/Delete/5
    public async Task<IActionResult> Delete(Guid id)
    {
        if (id == Guid.Empty)
        {
            return NotFound();
        }

        var student = await _context.Students
            .Include(s => s.Course)
            .FirstOrDefaultAsync(s => s.StudentID == id);
        if (student == null)
        {
            return NotFound("Student not found.");
        }

        return View(student);
    }

    // POST: Students/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student != null)
        {
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Student deleted successfully!";
        }
        return RedirectToAction(nameof(Index));
    }

    // Helper method to check student existence
    private async Task<bool> StudentExists(Guid id)
    {
        return await _context.Students.AnyAsync(e => e.StudentID == id);
    }
}
