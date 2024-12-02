using crud_system.Data;
using crud_system.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace crud_system.Controllers
{
    public class ChapterController : Controller
    {
        private readonly CourseManagementContext _context;

        public ChapterController(CourseManagementContext context)
        {
            _context = context;
        }

        // GET: Chapters
        public async Task<IActionResult> Index()
        {
            return View(await _context.Chapters.ToListAsync());
        }

        // GET: Chapters/Create
        public IActionResult Create()
        {
            // Pass courses for selection in the view
            ViewBag.Courses = _context.Courses.Select(c => new { c.CourseName }).ToList();
            return View();
        }

        // POST: Chapters/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ChapterID,CourseName,ChapterNumber")] Chapter chapter, IFormFile chapterVideo, IFormFile chapterPDF)
        {
            if (!_context.Courses.Any(c => c.CourseName == chapter.CourseName))
            {
                ModelState.AddModelError("CourseName", "Selected course does not exist.");
            }

            // Validate file uploads
            if (chapterVideo != null && !chapterVideo.ContentType.StartsWith("video/") && !chapterVideo.ContentType.Equals("image/gif"))
            {
                ModelState.AddModelError("ChapterVideo", "Only video files or GIFs are allowed.");
            }
            if (chapterPDF != null && !chapterPDF.ContentType.Equals("application/pdf"))
            {
                ModelState.AddModelError("ChapterPDF", "Only PDF files are allowed.");
            }

            if (ModelState.IsValid)
            {
                // Handle file uploads
                string videoFilePath = null;
                string pdfFilePath = null;

                if (chapterVideo != null)
                {
                    videoFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "videos", chapterVideo.FileName);
                    using (var fileStream = new FileStream(videoFilePath, FileMode.Create))
                    {
                        await chapterVideo.CopyToAsync(fileStream);
                    }
                    chapter.ChapterVideo = "/uploads/videos/" + chapterVideo.FileName;
                }

                if (chapterPDF != null)
                {
                    pdfFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "pdfs", chapterPDF.FileName);
                    using (var fileStream = new FileStream(pdfFilePath, FileMode.Create))
                    {
                        await chapterPDF.CopyToAsync(fileStream);
                    }
                    chapter.ChapterPDF = "/uploads/pdfs/" + chapterPDF.FileName;
                }

                chapter.ChapterID = Guid.NewGuid();
                _context.Add(chapter);

                // Increment chapter count for the course
                var course = _context.Courses.FirstOrDefault(c => c.CourseName == chapter.CourseName);
                if (course != null)
                {
                    course.CourseChapter++;
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Chapter added successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Courses = _context.Courses.Select(c => new { c.CourseName }).ToList();
            return View(chapter);
        }

        // GET: Chapters/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid Chapter ID.");
            }

            var chapter = await _context.Chapters.FindAsync(id);
            if (chapter == null)
            {
                return NotFound("Chapter not found.");
            }

            ViewBag.Courses = _context.Courses.Select(c => new { c.CourseName }).ToList();
            return View(chapter);
        }

        // POST: Chapters/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Chapter chapter, IFormFile chapterVideo, IFormFile chapterPDF)
        {
            if (id != chapter.ChapterID)
            {
                return BadRequest("Chapter ID mismatch.");
            }

            if (!_context.Courses.Any(c => c.CourseName == chapter.CourseName))
            {
                ModelState.AddModelError("CourseName", "Selected course does not exist.");
            }

            // Handle file uploads if new files are provided
            if (chapterVideo != null)
            {
                if (!chapterVideo.ContentType.StartsWith("video/") && !chapterVideo.ContentType.Equals("image/gif"))
                {
                    ModelState.AddModelError("ChapterVideo", "Only video files or GIFs are allowed.");
                }
                else
                {
                    string videoFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "videos", chapterVideo.FileName);
                    using (var fileStream = new FileStream(videoFilePath, FileMode.Create))
                    {
                        await chapterVideo.CopyToAsync(fileStream);
                    }
                    chapter.ChapterVideo = "/uploads/videos/" + chapterVideo.FileName;
                }
            }

            if (chapterPDF != null)
            {
                if (!chapterPDF.ContentType.Equals("application/pdf"))
                {
                    ModelState.AddModelError("ChapterPDF", "Only PDF files are allowed.");
                }
                else
                {
                    string pdfFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "pdfs", chapterPDF.FileName);
                    using (var fileStream = new FileStream(pdfFilePath, FileMode.Create))
                    {
                        await chapterPDF.CopyToAsync(fileStream);
                    }
                    chapter.ChapterPDF = "/uploads/pdfs/" + chapterPDF.FileName;
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chapter);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Chapter updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ChapterExists(chapter.ChapterID))
                    {
                        return NotFound("Chapter no longer exists.");
                    }
                    throw;
                }
            }

            ViewBag.Courses = _context.Courses.Select(c => new { c.CourseName }).ToList();
            return View(chapter);
        }

        // POST: Chapters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var chapter = await _context.Chapters.FindAsync(id);
            if (chapter != null)
            {
                var course = _context.Courses.FirstOrDefault(c => c.CourseName == chapter.CourseName);
                if (course != null)
                {
                    course.CourseChapter--;
                }

                _context.Chapters.Remove(chapter);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Chapter deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ChapterExists(Guid id)
        {
            return await _context.Chapters.AnyAsync(e => e.ChapterID == id);
        }
    }
}
