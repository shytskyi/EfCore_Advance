using EfCoreAdvance.DataBase;
using EfCoreAdvance.Entities;
using Microsoft.EntityFrameworkCore;
using CSharpFunctionalExtensions;

namespace EfCoreAdvance.Repositories
{
    public class StudentRepository
    {
        private readonly ApplicationDbContext _context;
        public StudentRepository(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<Result<Student>> GetById (long studentId)
        {
            var student = await _context.Students
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                .Include(s => s.FavoriteCourse)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null)
                return Result.Failure<Student>("Student not found");

            return student;
        }

        public void Save(Student student)
        {
            _context.Students.Attach(student);
        }
    }
}
