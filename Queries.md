# Quering Data using LING
In main in Program.cs .  
```
var context = new PlutoContext();

// LINQ syntax
var query =
    from c in context.Courses
    where c.name.Contains("c#")
    orderby c.Name
    select c;

foreach (var course in query)
    console.writeLine(course.Name);

// Extension methods
var courses = context.Courses
    .Where(c => c.Name.Contains("c#"))
    .OrderBy(c => c.Name);

foreach (var courses in courses)
    console.WriteLine(course.Name);
´´´
ctrl + F5  
### Diffrent Queries LINQ
´´´
// LINQ syntax
var query =
    from c in context.Courses
    where c.name.Contains("c#")
    orderby c.Name
    select c;

foreach (var course in query)
    console.writeLine(course.Name);

// Restrictions. Get all courses from level 1 and Author id 1.
var query =
    from c in context.Courses
    where c.Level == 1 && c.Author.Id == 1
    select c;

// Ordering. Order all courses by level.
var query =
    from c in context.Courses
    where c.Author.Id == 1
    // Multiple order by.
    orderby c.Level descending, c.Name
    select c;

// Projection. Return an object with properties.
var query =
    from c in context.Courses
    where c.Author.Id == 1
    // Multiple order by.
    orderby c.Level descending, c.Name
    // Anonymt object.
    select new { Name = c.Name, Author = c.Author.Name };

// Grouping. Group by level.
var query =
    from c in context.Courses
    group c by c.Level 
    into g
    select g;

foreach (var group in query) 
{
    Console.WriteLine(group.Key);
    foreach (var course in group)
        Console.WriteLine("\t{0}", course.Name);
}

// Agrete function. Counts with groups. Display all courses and the number of courses in each level.
var query =
    from c in context.Courses
    group c by c.Level 
    into g
    select g;

foreach (var group in query) 
{
    // 0 is the level and 1 is the number of courses in that level.
    Console.WriteLine("{0} ({1})", group.Key, group.Count());
    
}

// Joining. Link 2 tabels together.
// Inner join. Display list of courses and authors.
var query =
    from c in context.Courses
    // This is the wayto make a join if you need it. 
                    
    // We don't need to write join because link provider make it for us.
    select new { CourseName = c.Name, AuthorName = c.Author.Name };

    // When there is not a navigtion property or relation beteen 2 entities.
    // join a in context.Authors on c.AuthorId equals a.Id
    // select new { CourseName = c.Name, AuthorName = a.Name };

// Group joins. 
// Get all authors and count there courses.
var query =
    from a in context.Authors // left side.
    // The result of into g is a group join.
    join c in context.Courses on a.Id equals c.AuthorId into g // right side.
    select new { AuthorName = a.Name, Courses = g.Count() };
    
    foreach (var x in query)
    // 0 is the author and 1 is the count of courses.
    Console.WriteLine("{0} ({1})", x.AuthorName, x.Courses);

// Cross join.
var query =
    from a in context.Authors
    from c in context.Courses
    select new { AuthorName = a.Name, CourseName = c.Name };

foreach (var x in query)
    // 0 is the author name and 1 is the course name.
    Console.WriteLine("{0} - ({1})", x.AuthorName, x.CourseName);
´´´
### Diffrent queries extension
´´´

´´´
