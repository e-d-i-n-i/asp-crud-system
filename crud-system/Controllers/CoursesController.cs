using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using crud_system.Models;
using crud_system.Data;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class CoursesController : Controller
{
    private readonly CourseManagementContext _context;

    public CoursesController(CourseManagementContext context)
    {
        _context = context;
    }

    
    // GET: Courses
    public async Task<IActionResult> Index()
    {
        return View(await _context.Courses.ToListAsync());
    }

    // GET: Courses/Create
    public IActionResult Create()
    {
        return View();
    }
    // POST: Courses/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("CourseID,CourseName,CourseChapter")] Course course)
    {
        // Check if the course name already exists in the database
        if (_context.Courses.Any(c => c.CourseName == course.CourseName))
        {
            ModelState.AddModelError("CourseName", "A course with this name already exists.");
            return View(course); // Return the view with the error message
        }

        if (ModelState.IsValid)
        {
            course.CourseID = Guid.NewGuid(); // Set the CourseID
            _context.Add(course);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Course Added successfully!";
            return RedirectToAction(nameof(Index));
        }
        return View(course);
    }


    // GET: Courses/Edit/5
    public async Task<IActionResult> Edit(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest("Invalid course ID.");
        }

        var course = await _context.Courses.FindAsync(id);
        if (course == null)
        {
            return NotFound("Course not found.");
        }

        return View(course);
    }

    // POST: Courses/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Course course)
    {
        if (id != course.CourseID)
        {
            return BadRequest("Course ID mismatch.");
        }

        // Check if the course name already exists in the database (excluding the current course)
        if (_context.Courses.Any(c => c.CourseName == course.CourseName && c.CourseID != id))
        {
            ModelState.AddModelError("CourseName", "A course with this name already exists.");
            return View(course); // Return the view with the error message
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Entry(course).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CourseExists(course.CourseID))
                {
                    return NotFound("Course no longer exists.");
                }
                throw;
            }

            TempData["SuccessMessage"] = "Course updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        return View(course);
    }


    // Helper method to check course existence
    private async Task<bool> CourseExists(Guid id)
    {
        return await _context.Courses.AnyAsync(e => e.CourseID == id);
    }

    // GET: Courses/Delete/5
    public async Task<IActionResult> Delete(Guid id)
    {
        if (id == Guid.Empty)
        {
            return NotFound();
        }

        var course = await _context.Courses
            .FirstOrDefaultAsync(m => m.CourseID == id);
        if (course == null)
        {
            return NotFound();
        }

        return View(course);
    }

    // POST: Courses/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course != null)
        {
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }
        TempData["SuccessMessage"] = "Course Deleted successfully!";
        return RedirectToAction(nameof(Index));
    }

    
    

}
