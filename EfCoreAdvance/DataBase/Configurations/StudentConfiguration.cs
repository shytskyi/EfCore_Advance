using EfCoreAdvance.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCoreAdvance.DataBase.Configurations
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.ToTable("Students").HasKey(s => s.Id);
            builder.Property(s => s.Id).HasColumnName("StudentID");
            builder.OwnsOne(s => s.Name, nameBuilder =>
            {
                nameBuilder.Property(f => f.First).HasColumnName("Firstname").HasMaxLength(100);
                nameBuilder.Property(l => l.Last).HasColumnName("Lastname").HasMaxLength(100);
            });
            builder.HasOne(s => s.FavoriteCourse)
                .WithMany();
            builder.HasMany(s => s.Enrollments)
                .WithOne(e => e.Student);
        }
    }
}
