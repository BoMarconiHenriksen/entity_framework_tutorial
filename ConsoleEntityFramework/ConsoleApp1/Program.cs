using System.Collections.Generic;
using System.Data.Entity;

namespace CodeFirst
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public CourseLevel Level { get; set; }
        public float FullPrice { get; set; }
        public Author Author { get; set; }
        public IList<Tag> Tags { get; set; }
    }

    // En til mange relation mellem Author og Courses. Course listen er i Author.
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<Course> Courses { get; set; }
    }

    // Mange til mange relation mellem Course og Tag. Liste i begge klasser.
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<Course> Courses { get; set; }
    }

    public enum CourseLevel
    {
        Beginner = 1,
        Intermediate = 2,
        Advanced = 3
    }

    // DbContext abstraction for db connection, læs/skrive til databasen, transaction osv.
    public class PlutoContext : DbContext
    {
        // En samling af objekter, der hører til en tabel i databasen.
        // Kan f.eks. tilgå Courses tabellen i databasen.
        public DbSet<Course> Courses { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public PlutoContext()
            : base("name=DefaultConnection")
         {

        }
    }

    class Program
    {
        static void Main(string[] args)
        {

        }
    }
}
