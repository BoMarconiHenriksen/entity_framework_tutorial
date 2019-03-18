# Web API 2 with Entity Framework Tutorial
https://docs.microsoft.com/en-us/aspnet/web-api/overview/data/using-web-api-with-entity-framework/part-1  

### Software versions used in the tutorial
Web API 2.1  
Visual Studio Code  
Entity Framework 6  
.NET 4.7  
Knockout.js 3.1  

### Here are the main building blocks for this app:
ASP.NET MVC creates the HTML page.  
ASP.NET Web API handles the AJAX requests and returns JSON data.  
Knockout.js data-binds the HTML elements to the JSON data.  
Entity Framework talks to the database.  

### Create Project
1. New Project ASP.NET Web Application (.NET Framework) and then web API.  
2. Configure Azure settings or database settings.  

### Add Model Classes (Code First)
1. Add domain objects in the Model folder(the domain objects will be used to create the tables in the database).  
   You can use the navigation property(in the Book class Ather Auther) to access the related Author in code.  

### Add Web API Controllers (CRUD)
The controllers will use Entity Framework to communicate with the database layer.  

1. In the folder Controllers delete ValueController.cs  
2. Build the project.  
3. Add a Controller. select "Web API 2 Controller with actions, using Entity Framework".  
   In the Model class dropdown, select the Author class. (If you don't see it listed in the dropdown, make sure that you built the project.)
   Check "Use async controller actions".  
   Leave the controller name as "AuthorsController".  
   Click plus (+) button next to Data Context Class.  
   In the New Data Context dialog, leave the default name and click Add.  
   Click Add to complete the Add Controller dialog. The dialog adds two classes to your project: 
	- AuthorsController defines a Web API controller. The controller implements the REST API that clients use to perform CRUD operations on the list of authors.  
	- BookServiceContext manages entity objects during run time, which includes populating objects with data from a database, change tracking, and persisting data to the database. It inherits from DbContext.  
4. Build the project.  
5. Now go through the same steps to add an API controller for Book entities. This time, select Book for the model class, and select the existing BookServiceContext class for the data context class. (Don't create a new data context.) Click Add to add the controller.  
   Model class: Book(BookService.Models)  
	Data context class: BookServiceContext (BookService.Moldes)  

### Use Code First Migrations to Seed the Database
Alt t n o for package manger console.  
1. Enable-Migrations (This command adds a folder named Migrations to your project, plus a code file named Configuration.cs in the Migrations folder.)  
2. Seed the Configuration.cs file if needed. Remember to add the namespace.  
3. Add-Migration Initial (generates code that creates the database).  
4. Update-Database (executes that code. The database is created locally, using LocalDB.)  

### Explore the API
Press F5.  

### View the Database
You can view the local database in Visual Studio.  
1. From the View menu, select SQL Server Object Explorer.  

### Handling Entity Relations
To trace the SQL, add the following line of code to the BookServiceContext constructor:  
```
public BookServiceContext() : base("name=BookServiceContext")
{
    // New code:
    this.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
}
```
There are three ways to load related data in Entity Framework: eager loading, lazy loading, and explicit loading.  
#### Eager Loading
With eager loading, EF loads related entities as part of the initial database query. To perform eager loading, use the System.Data.Entity.Include extension method. In BooksController.cs  
```
public IQueryable<Book> GetBooks()
{
    return db.Books
        // new code:
        .Include(b => b.Author);
}
```
This tells EF to include the Author data in the query.  
The trace log shows that EF performed a join on the Book and Author tables.  
#### Lazy Loading
With lazy loading, EF automatically loads a related entity when the navigation property for that entity is dereferenced. To enable lazy loading, make the navigation property virtual. For example, in the Book class:  
```
public class Book
{
    // (Other properties)

    // Virtual navigation property
    public virtual Author Author { get; set; }
}
```
Now consider the following code:  
```
var books = db.Books.ToList();  // Does not load authors
var author = books[0].Author;   // Loads the author for books[0]
```
When lazy loading is enabled, accessing the Author property on ```books[0]``` causes EF to query the database for the author.  

Lazy loading requires multiple database trips, because EF sends a query each time it retrieves a related entity. Generally, you want lazy loading disabled for objects that you serialize. The serializer has to read all of the properties on the model, which triggers loading the related entities.  

EF makes three separate queries for the three authors.  

There are still times when you might want to use lazy loading. Eager loading can cause EF to generate a very complex join. Or you might need related entities for a small subset of the data, and lazy loading would be more efficient.  

One way to avoid serialization problems is to serialize data transfer objects (DTOs) instead of entity objects. I'll show this approach later in the article.  
#### Explicit Loading
Explicit loading is similar to lazy loading, except that you explicitly get the related data in code; it doesn't happen automatically when you access a navigation property. Explicit loading gives you more control over when to load related data, but requires extra code. https://docs.microsoft.com/en-us/ef/ef6/querying/related-data#explicit  
### Navigation Properties and Circular References
When I defined the Book and Author models, I defined a navigation property on the Book class for the Book-Author relationship, but I did not define a navigation property in the other direction.  

What happens if you add the corresponding navigation property to the Author class?  
```
public class Author
{
    public int AuthorId { get; set; }
    [Required]
    public string Name { get; set; }

    public ICollection<Book> Books { get; set; }
}
```
Unfortunately, this creates a problem when you serialize the models. If you load the related data, it creates a circular object graph. https://docs.microsoft.com/en-us/aspnet/web-api/overview/data/using-web-api-with-entity-framework/part-4  
One solution is to use DTOs.  
### Create Data Transfer Objects(DTOs)
Right now, our web API exposes the database entities to the client. The client receives data that maps directly to your database tables. However, that's not always a good idea. Sometimes you want to change the shape of the data that you send to client. For example, you might want to:  

1. Remove circular references (see previous section).
2. Hide particular properties that clients are not supposed to view.
3. Omit some properties in order to reduce payload size.
4. Flatten object graphs that contain nested objects, to make them more convenient for clients.
5. Avoid "over-posting" vulnerabilities. (See Model Validation for a discussion of over-posting.)
6. Decouple your service layer from your database layer.
To accomplish this, you can define a data transfer object (DTO). A DTO is an object that defines how the data will be sent over the network. Let's see how that works with the Book entity. In the Models folder, add two DTO classes:  
```
namespace BookService.Models
{
    public class BookDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string AuthorName { get; set; }
    }
}

namespace BookService.Models
{
    public class BookDetailDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public decimal Price { get; set; }
        public string AuthorName { get; set; }
        public string Genre { get; set; }
    }
}
```
The BookDetailDTO class includes all of the properties from the Book model, except that AuthorName is a string that will hold the author name. The BookDTO class contains a subset of properties from BookDetailDTO.  

Next, replace the two GET methods in the BooksController class, with versions that return DTOs. We'll use the LINQ Select statement to convert from Book entities into DTOs.  
```
// GET api/Books
public IQueryable<BookDTO> GetBooks()
{
    var books = from b in db.Books
                select new BookDTO()
                {
                    Id = b.Id,
                    Title = b.Title,
                    AuthorName = b.Author.Name
                };

    return books;
}

// GET api/Books/5
[ResponseType(typeof(BookDetailDTO))]
public async Task<IHttpActionResult> GetBook(int id)
{
    var book = await db.Books.Include(b => b.Author).Select(b =>
        new BookDetailDTO()
        {
            Id = b.Id,
            Title = b.Title,
            Year = b.Year,
            Price = b.Price,
            AuthorName = b.Author.Name,
            Genre = b.Genre
        }).SingleOrDefaultAsync(b => b.Id == id);
    if (book == null)
    {
        return NotFound();
    }

    return Ok(book);
}
```
Finally, modify the PostBook method to return a DTO.  
```
[ResponseType(typeof(BookDTO))]
public async Task<IHttpActionResult> PostBook(Book book)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    db.Books.Add(book);
    await db.SaveChangesAsync();

    // New code:
    // Load author name
    db.Entry(book).Reference(x => x.Author).Load();

    var dto = new BookDTO()
    {
        Id = book.Id,
        Title = book.Title,
        AuthorName = book.Author.Name
    };

    return CreatedAtRoute("DefaultApi", new { id = book.Id }, dto);
}
```
In this tutorial, we're converting to DTOs manually in code. Another option is to use a library like AutoMapper that handles the conversion automatically.  
### Create the JavaScript Client
In this section, you will create the client for the application, using HTML, JavaScript, and the Knockout.js library. We'll build the client app in stages:  

1. Showing a list of books.  
2. Showing a book detail.  
3. Adding a new book.  

1. The model is the server-side representation of the data in the business domain (in our case, books and authors).  
2. The view is the presentation layer (HTML).  
3. The view model is a JavaScript object that holds the models. The view model is a code abstraction of the UI. It has no knowledge of the HTML representation. Instead, it represents abstract features of the view, such as "a list of books".  

The view is data-bound to the view model. Updates to the view model are automatically reflected in the view. The view model also gets events from the view, such as button clicks. Picture https://docs.microsoft.com/en-us/aspnet/web-api/overview/data/using-web-api-with-entity-framework/part-6  

This approach makes it easy to change the layout and UI of your app, because you can change the bindings, without rewriting any code. For example, you might show a list of items as a <ul>, then change it later to a table.  
#### Add the Knockout Library
Install-Package knockoutjs (adds the Knockout files to the Scripts folder)  
In the script folder add a app.js file and paste the code in.  
### Add a Script Bundle
Bundling is a feature in ASP.NET 4.5 that makes it easy to combine or bundle multiple files into a single file. Bundling reduces the number of requests to the server, which can improve page load time.  

Open the file App_Start/BundleConfig.cs. Add the following code to the RegisterBundles method.  
```
public static void RegisterBundles(BundleCollection bundles)
{
    // ...

    // New code:
    bundles.Add(new ScriptBundle("~/bundles/app").Include(
              "~/Scripts/knockout-{version}.js",
              "~/Scripts/app.js"));
}
```
### Create the view
Change the view in the view folder.  
### Display Item Details
https://docs.microsoft.com/en-us/aspnet/web-api/overview/data/using-web-api-with-entity-framework/part-8  
### Add a New Item to the Database
https://docs.microsoft.com/en-us/aspnet/web-api/overview/data/using-web-api-with-entity-framework/part-9  
### Publish to Azure Cloud
https://docs.microsoft.com/en-us/aspnet/web-api/overview/data/using-web-api-with-entity-framework/part-10  
