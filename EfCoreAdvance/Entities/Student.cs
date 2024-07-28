using CSharpFunctionalExtensions;

namespace EfCoreAdvance.Entities
{
    public class Student : Entity
    {
        public Name Name { get; set; } = null!;
        public Course? FavoriteCourse { get; set; } 
        public List<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

        public Student(Name name, Course? course)
        {
            Name = name;
            FavoriteCourse = course;
        }
        private Student() { }
    }
}
