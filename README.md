# Entity Framework Tutorial for ConsoleApp
For vs code use the extension Dotnet core commands. F1. dotnet.  

https://docs.microsoft.com/en-us/ef/core/get-started/install/  
### Create a database with code first
1. Install Entity Framework via package manager console. Tools --> Nuget Packet Manager --> Package Manager Console(tryk Alt t n o) --> install-package EntityFramework  
2. In Program.cs create your entities.  
3. Create a dbContext.  
4. Specify the connection string in App.config. If you use the same name as your dbContext you don't need to specify any additional configuration.  
5. Enable migration from package manager console: enable-migrations (Only once in a project.).  
6. Add migration from package manager console: add-migration ```<name>```
   In the migration folder you can find the migration.  
7. To run it: From the package manager console: update-database  
### Importing database and then use code first
1. In solution manager right click on project name.  
2. Then add --> New Item --> Giv it a name and Select ADO.Net Entity Model.  
3. Vælg database.  
4. Vælg Microsoft SQL Server (SqlClient) og i server name: .\SQLEXPRESS, select database and test connection.  
5. In tables vælg alle tables and be sure NOT to chooce MigrationHistory.  
6. Check the name of your classes.  
7. enable-migrations  
8. add-migration InitialModel DOG LOOK AT 9  
9. If you have an exisiting database then use ignore changes first time you run migration. ##### add-migration InitialModel -IgnoreChanges -Force  
10. You can only have one migration at a time so you need to update the database with the empty migration. ##### Update-Database
### Work with database(change model) with code first
1. Create a new class.  
2. Add the class to PlutoContext.cs. ```public virtual DbSet<Category> Categories { get; set; }```
3. add-migration ```<name>``` (use -force to overwrite an migration).
4. Populate a table with a sql mapper. I migration: Sql("INSERT INTO Categories VALUES (1, 'Web Development')");  
5. update-database  
### Modifying an existing class
#### Adding a new property
1. Add public DateTime? DatePublished { get; set; } to a class.  
2. add-migration AddDatePublishedColumnToCoursesTable  
3. Update-Database  
#### Modifying an existing property
1. Change this to Name: public string Name { get; set; }  
2. add-migration RenameTitleToNameInCoursesTable  
3. Be sure that you copy or use rename so you don't loose data. Sql("UPDATE Courses SET Name = Title"); or RenameColumn("dbo.Courses", "Title", "Name");  
4.  Check also the down method.  Add this(Byt rundt på name og title) Sql("UPDATE Courses SET Title = Name"); before dropping the table.  
5. Update-Database  
#### Deleting an existing property
1. Delete this: public DateTime? DatePublished { get; set; }  
2. add-migration DeleteDataPublishedColumnFromCoursesTable  
3. Update-Database  
### Deleting an existing class
1. Delete the class itself and the properties in other classes.  
2. First start by deleting the property and then make a migration and update the database.  
3. Delete the class and the Category in PlutoContext.  
4. Make a migration and update the database.  
5. If you want to perse the data in the table. In the migration file up method:  
   ```
    CreateTable(
        "dbo.Categories",
            c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(),
                })
            .PrimaryKey(t => t.Id);

            Sql("INSERT INTO _Categories (Name) SELECT Name FROM Categories");

            DropTable("dbo.Categories");
   ```
   Then in the down method:  
   ```
    CreateTable(
        "dbo.Categories",
        c => new
            {
                Id = c.Int(nullable: false, identity: true),
                Name = c.String(),
            })
        .PrimaryKey(t => t.Id);

        Sql("INSERT INTO Categories (Name) SELECT Name FROM _Categories");

        DropTable("dbo._Categories");
   ```
6. Update-database  
### Recovering from mistakes
Find the migration from the history and make a new migration.  
### Downgrading a Database
Update-Database -TargetMigration:F  
It then run the down methods.  
1. Update-Database -TargetMigration:DeleteDataPublishedColumnFromCoursesTable  
2. After you have made your changes you can bring it back to the latest version: Update-Database  
### Seeding database in Configuration.cs
Use the seed method.  
1. add-migration PopulateCategoriesTable
2. Make the seed method.  
```
protected override void Seed(CodeFirstFromExistingDb.PlutoContext context)
    {
        context.Authors.AddOrUpdate(a => a.Name, 
            new Author
            {
                Name = "Author 1",
                Courses= new Collection<Course>()
                {
                    new Course
                    {
                        Name = "Course for auther 1", Description = "Description"
                    }
                }
            });
    }
```
3. Update-Database











