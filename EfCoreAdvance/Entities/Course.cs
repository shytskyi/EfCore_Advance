using CSharpFunctionalExtensions;

namespace EfCoreAdvance.Entities
{
    public class Course : Entity
    {
        public static readonly Course Math = new Course(1, "Math");
        public static readonly Course Chemistry = new Course(2, "Chemistry");
        public static readonly Course[] AllCourses = { Math, Chemistry };

        public string Name { get; } = null!;

        private Course() { }

        private Course(long id, string name) : base(id)
        {
            this.Name = name;
        }

        public static Result<Course> FromId(long id)
        {
            var course = AllCourses.SingleOrDefault(x => x.Id == id);

            if (course is null)
            {
                return Result.Failure<Course>("Course not found");
            }

            return course;
        }
    }
}
