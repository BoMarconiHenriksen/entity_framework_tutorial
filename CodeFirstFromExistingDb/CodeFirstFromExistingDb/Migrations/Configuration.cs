namespace CodeFirstFromExistingDb.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CodeFirstFromExistingDb.PlutoContext>
    {

        protected override void Seed(CodeFirstFromExistingDb.PlutoContext context)
        {
            context.Authors.AddOrUpdate(a => a.Name, 
                new Author
                {
                    Name = "Author 1",
                    Courses = new Collection<Course>()
                    {
                        new Course
                        {
                            Name = "Course for auther 1", Description = "Description"
                        }
                    }
                });
        }
    }
}
