using EfCoreAdvance.DataBase.Configurations;
using EfCoreAdvance.Entities;
using Microsoft.EntityFrameworkCore;

namespace EfCoreAdvance.DataBase
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }

        #region The first method to connect database + code in Program.cs

        //public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :base(options) { }
        #endregion


        #region The second method to connect database

        private readonly IConfiguration configuration;
        public ApplicationDbContext(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer(configuration.GetConnectionString("Database"))
                .UseLoggerFactory(CreateLoggerFactory()) //tak zeby byly tylko logi do DB w appsetings.json dodajemy ("Microsoft.EntityFrameworkCore.Database": "Information")
                .EnableSensitiveDataLogging();  //jesli chcesz widziec log zapytow do database
        }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.ApplyConfiguration(new StudentConfiguration());   jedna kofiguracja

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly); //wszystkie konfiguracje wykonane podobne do StudentConfiguration 
                                                                                                 //w danym wypadku tylko jedna kofiguracja

            modelBuilder.Entity<Course>(courseBuilder =>
            {
                courseBuilder.ToTable("Courses").HasKey(s => s.Id);
                courseBuilder.Property(s => s.Id).HasColumnName("CourseID");
                courseBuilder.Property(s => s.Name).HasMaxLength(100);

                courseBuilder.HasData(Course.Math, Course.Chemistry);
            });

            modelBuilder.Entity<Enrollment>(enrollmentBuilder =>
            {
                enrollmentBuilder.ToTable("Enrollments").HasKey(s => s.Id);
                enrollmentBuilder.Property(s => s.Id).HasColumnName("EnrollmentID");
                enrollmentBuilder.HasOne(e => e.Student).WithMany(s => s.Enrollments);
                enrollmentBuilder.HasOne(e => e.Course).WithMany();
                enrollmentBuilder.Property(e => e.Grade).HasMaxLength(100);
            });
        }

        public ILoggerFactory CreateLoggerFactory()
        {
            return LoggerFactory.Create(builder => { builder.AddConsole(); });
        }
    }
}


//  Add-Migration InitialCreate                      - nowa (pirwsza) migracja
//  Update-Database                                  - stosowanie migracji do DB
//  Remove-Migration                                 - usunoc ostatniu migracju
//  Update-Database -Migration AddEmailToStudent     - roll back to InitialCreate
//  Drop-Database                                    - delete DB
//  Script-Migration                                 - generacja SQL-skryptu dla migracji