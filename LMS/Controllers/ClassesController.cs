using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LMS.Data;
using Microsoft.AspNetCore.Authorization;
using LMS.Models;

namespace LMS.controllers
{
    [Authorize]
    public class ClassesController : Controller
    {
        private readonly LmsDbContext _context;

        public ClassesController(LmsDbContext context)
        {
            _context = context;
        }

        // GET: Classes
        public async Task<IActionResult> Index()
        {
            var lmsDbContext = _context.Classes
                .Include(c => c.Course)
                .Include(c => c.Lecturer);
            return View(await lmsDbContext.ToListAsync());
        }

        // GET: Classes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Classes == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes
                .Include(c => c.Course)
                .Include(c => c.Lecturer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@class == null)
            {
                return NotFound();
            }

            return View(@class);
        }

        // GET: Classes/Create
        public IActionResult Create()
        {
            var courses = _context.Courses.Select( q => new
            {
                CourseDetails = $"{q.Code} - {q.Name}({q.Credits})",
                q.Id
            }
            );
            ViewData["CourseId"] = new SelectList(courses, "Id", "CourseDetails");
            var lecturers = _context.Lecturers.Select(q => new 
            {
                FullName = $"{q.FirstName} {q.LastName}",
                q.Id
            });
            ViewData["LecturerId"] = new SelectList(lecturers, "Id", "FullName");
            return View();
        }

        // POST: Classes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LecturerId,CourseId,Time")] Class @class)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@class);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            CreateSelectLists();
            return View(@class);
        }

        // GET: Classes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Classes == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes.FindAsync(id);
            if (@class == null)
            {
                return NotFound();
            }
            CreateSelectLists();
            return View(@class);
        }

        // POST: Classes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LecturerId,CourseId,Time")] Class @class)
        {
            if (id != @class.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@class);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassExists(@class.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            CreateSelectLists();
            return View(@class);
        }

        // GET: Classes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Classes == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes
                .Include(c => c.Course)
                .Include(c => c.Lecturer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@class == null)
            {
                return NotFound();
            }

            return View(@class);
        }

        // POST: Classes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Classes == null)
            {
                return Problem("Entity set 'LmsDbContext.Classes'  is null.");
            }
            var @class = await _context.Classes.FindAsync(id);
            if (@class != null)
            {
                _context.Classes.Remove(@class);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<ActionResult> ManageEnrollments(int id)
        {
            var @class = await _context.Classes
                .Include(q => q.Course)
                .Include(q => q.Lecturer)
                .Include(q => q.Enrollments)
                   .ThenInclude(q => q.Student)
                .FirstOrDefaultAsync(m => m.Id == id);

            var students = await _context.Students.ToListAsync();

            var model = new ClassEnrollmentViewModel();
            model.Class = @class;

            foreach(var student in students)
            {
                model.Students.Add(new StudentEnrollmentViewModel
                {
                    Id = student.Id,
                    FirstName = student.FirstName,
                    LastName = student.LastName,
                    IsEnrolled = (@class?.Enrollments?.Any(q => q.StudentId == student.Id))
                                 .GetValueOrDefault()
                });
            }

            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnrollStudent(int classId, int studentId, bool shouldEnroll)
        {
            var enrollment = new Enrollment();
            if(shouldEnroll)
            {
                enrollment.ClassId = classId;
                enrollment.StudentId = studentId;
                await _context.AddAsync(enrollment);
            }
            else
            {
                enrollment = await _context.Enrollments.FirstOrDefaultAsync(
                    q => q.ClassId == classId && q.StudentId == studentId
                    );

                if(enrollment != null)
                {
                    _context.Remove(enrollment);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        private bool ClassExists(int id)
        {
          return (_context.Classes?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private void CreateSelectLists()
        {
            var courses = _context.Courses.Select(q => new
            {
                CourseDetails = $"{q.Code} - {q.Name}{(q.Credits)}",
                q.Id
            }
          );
            ViewData["CourseId"] = new SelectList(courses, "Id", "CourseDetails");
            var lecturers = _context.Lecturers.Select(q => new
            {
                FullName = $"{q.FirstName} {q.LastName}",
                q.Id
            });
            ViewData["LecturerId"] = new SelectList(lecturers, "Id", "FullName");
        }
    }
}
