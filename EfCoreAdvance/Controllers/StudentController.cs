using EfCoreAdvance.DataBase;
using EfCoreAdvance.Entities;
using EfCoreAdvance.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EfCoreAdvance.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly StudentRepository _studentRepository;

        public StudentController(ApplicationDbContext dbContext, StudentRepository studentRepository)
        {
            _dbContext = dbContext;
            _studentRepository = studentRepository;
        }

        //// [HttpGet("favorite-course")]
        //// public async Task<ActionResult<string>> CheckStudentFavoriteCourse(long studentId, long courseId)
        //// {
        //// }
        ////
        [HttpPut("enroll")]
        public async Task<ActionResult<string>> EnrollStudent(long studentId, long courseId, Grade grade)
        {
            var studentResult = await _studentRepository.GetById(studentId);
            if (studentResult.IsFailure)
                return BadRequest(studentResult.Error);

            var course = await _dbContext.Courses.FindAsync(courseId);

            if (course is null)
                return BadRequest("Course not found");

            var enrollment = new Enrollment(course, studentResult.Value, grade);

            if (studentResult.Value.Enrollments.Any(e => e.Course == course))
            {
                return BadRequest("Enrollment already exist");
            }

            studentResult.Value.Enrollments.Add(enrollment);

            await _dbContext.SaveChangesAsync();

            return Ok("Ok");
        }


        [HttpPut("disenroll")]
        public async Task<ActionResult<string>> DisenrollStudent(long studentId, long courseId)
        {
            var studentResult = await _studentRepository.GetById(studentId);
            if (studentResult.IsFailure)
                return BadRequest(studentResult.Error);

            var course = await _dbContext.Courses.FindAsync(courseId);

            if (course is null)
                return BadRequest("Course not found");

            var enrollment = studentResult.Value.Enrollments
                .FirstOrDefault(x => x.Course == course);

            if (enrollment is null)
                return BadRequest("No enrollment");

            studentResult.Value.Enrollments.Remove(enrollment);

            var entries = _dbContext.ChangeTracker.Entries();

            await _dbContext.SaveChangesAsync();

            return Ok("OK");
        }

        [HttpPost("register")]
        public async Task<ActionResult<string>> RegisterStudent(
            string firstName,
            string lastName,
            long favoriteCourseId,
            Grade favoriteCourseGrade)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var nameResult = Name.Create(firstName, lastName);
                if (nameResult.IsFailure)
                    return BadRequest(nameResult.Error);

                var course1 = await _dbContext.Courses.FindAsync(favoriteCourseId);

                if (course1 is null)
                    return BadRequest("Course not found");

                var student = new Student(nameResult.Value, course1);

                _dbContext.Students.Attach(student);

                //var entrise = _dbContext.ChangeTracker.Entries();

                await _dbContext.SaveChangesAsync();

                var enrollment = new Enrollment(course1, student, favoriteCourseGrade);

                student.Enrollments.Add(enrollment);

                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
            }

            return Ok("Ok");
        }

        [HttpPut("edit")]
        public async Task<ActionResult<string>> EditPersonalInfo(
            long studentId,
            string firstName,
            string lastName,
            long favoriteCourseId)
        {
            var student = await _dbContext.Students.FindAsync(studentId);

            if (student is null)
                return BadRequest("Student not found");

            var nameResult = Name.Create(firstName, lastName);
            if (nameResult.IsFailure)
                return BadRequest(nameResult.Error);

            var course = await _dbContext.Courses.FindAsync(favoriteCourseId);

            if (course is null)
                return BadRequest("Course not found");

            student.Name = nameResult.Value;
            student.FavoriteCourse = course;

            await _dbContext.SaveChangesAsync();

            return Ok("ОК");
        }
    }
}
