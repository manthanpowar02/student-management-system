using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EnrollmentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public EnrollmentsController(AppDbContext context) => _context = context;

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Enroll([FromBody] Enrollment enrollment)
        {
            var studentExists = await _context.Students.AnyAsync(s => s.Id == enrollment.StudentId);
            var courseExists = await _context.Courses.AnyAsync(c => c.Id == enrollment.CourseId);
            if (!studentExists || !courseExists)
                return BadRequest("Student or Course not found");

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Student enrolled successfully" });
        }

        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetByStudent(int studentId)
        {
            var enrollments = await _context.Enrollments
                .Where(e => e.StudentId == studentId)
                .Include(e => e.Course)
                .ToListAsync();
            return Ok(enrollments);
        }
    }
}