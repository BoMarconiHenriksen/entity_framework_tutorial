# Entity Framework Core for .Net Core 2.2
https://code-maze.com/net-core-web-api-ef-core-code-first/  
https://code-maze.com/net-core-web-development-part2/#creatingNewProject  

1. Open Visual Studio 2017
2. File > New > Project
3. From the left menu, select Installed > Visual C# > .NET Core.
4. Select ASP.NET Core Web Application.
5. Enter ´´´<name>``` for the name and click OK.
6. In the New ASP.NET Core Web Application dialog:
7. Make sure that .NET Core and ASP.NET Core 2.2 are selected in the drop-down lists
8. Select the API project template
9. Make sure that Authentication is set to No Authentication
10. Click OK

Change the applicationUrl property and the launchBrowser property to false, to prevent web browser to start when the project starts.  
In the Solution Explorer expand the Properties and double click on the launchSettings.json.  
```
In all places change to:
"launchBrowser": false
```
#### Program.cs
The Startup class is a mandatory for the .NET Core, in which we configure embedded or custom services that our application needs. When you open the Startup class, you can see the constructor and the two methods.  
In the method ConfigureServices, you will do exactly that, configure your services.  
Furthermore, in the method Configure you are going to add different middleware components to the application’s request pipeline.  

All of our configuration code could be written inside the ConfigureServices method, but large applications could potentially contain many services. As a result, it could make this method unreadable and hard to maintain. Therefore we will create extension methods for each configuration and place the configuration code inside those methods.  

### Install Entity Framework Core
1. Install-Package Microsoft.EntityFrameworkCore  
2. (Build project)

### Create the model
1. Right-click on the Models folder and select Add > Class.  
2. (Rebuild project).  

### Extensions Methods
Create a folder in the root called extensions and make a ServiceExtensions class.  
```
public static class ServiceExtensions
    {
        // For CORS
        //public static void ConfigureCors(this IServiceCollection services)
        //{
        //    services.AddCors(options =>
        //    {
        //        options.AddPolicy("CorsPolicy",
        //            builder => builder.AllowAnyOrigin()
        //            .AllowAnyMethod()
        //            .AllowAnyHeader()
        //            .AllowCredentials());
        //    });
        //}

        // configure an IIS integration which will help us with the IIS deployment. 
        public static void ConfigureIISIntegration(this IServiceCollection services)
        {
            services.Configure<IISOptions>(options =>
            {

            });
        }
    }
```
In the Startup class we will change the ConfigureServices and Configure methods:  
```
https://code-maze.com/net-core-web-development-part2/#creatingNewProject
```
Define the database connection in the appsettings.json file as:  
```

```


#### Create a Context File
https://code-maze.com/net-core-web-development-part4/#context  
Now, let us create the context class, which will be a middleware component for the communication with the database. It has DbSet properties which contain the table data from the database.  

In the root of Entities project create the RepositoryContext class and modify it:  
```
using Entities.Models;
using Microsoft.EntityFrameworkCore;
 
namespace Entities
{
    public class RepositoryContext: DbContext
    {
        public RepositoryContext(DbContextOptions options)
            :base(options)
        {
        }
 
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Account> Accounts { get; set; }
    }
}
```
Remember to install third-party library via NuGet pacakage manager.  
For sql Express.  
```Install-Package Microsoft.EntityFrameworkCore.SqlServer```

Define the database connection:  
```
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "ConnectionString": {
    "DefaultConnection": "Data Source=.\\\\\\SQLEXPRESS;Initial Catalog=c3NextGenDB;Integrated Security=True;MultipleActiveResultSets=True"
  },
  "AllowedHosts": "*"
}

// "Data Source=.\\\SQLEXPRESS;Initial Catalog=ContactsDB;Integrated Security=True;MultipleActiveResultSets=True"
// <add name="PlutoContext" connectionString="data source=.\SQLEXPRESS;initial catalog=Pluto_DataAnnotations;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework" providerName="System.Data.SqlClient" />
// server=MY_SERVER;database=EmployeeDB;User ID=MY_USER;password=MY_PASSWORD;
// server=localhost;userid=root;password=yourpass;database=accountowner;
```
In the ServiceExtensions class, you are going to write the code for configuring the MySQL context.  

First, add the using directives and then add the method ConfigureMySqlContext:  
```
public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration config)
{
	// access the appsettings.json
	var connectionString = config["ConnectionString:DefaultConnection"];
	services.AddDbContext<RepositoryContext>(options => options.UseSqlServer(connectionString));
}
```
In the Startup class in the ConfigureServices method, add the context service to the IOC right above the services.AddMvc():  
```
services.ConfigureSqlContext(Configuration);
```
If you have 2 foreign keys in a tabel add this to your context file.  
```
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
	modelBuilder.Entity<FormFieldValue>()
		.HasOne(f => f.FormField)
		.WithMany()
		.OnDelete(DeleteBehavior.Restrict);
}
```




### Create a controller
Remember to rebuild the project.  

### Register the context with dependency injection
To make ```<nameContext>``` available to MVC controllers, register it as a service.  
In Startup.cs add the following using statements:  
```
using <nameOfProject>.Models;
using Microsoft.EntityFrameworkCore;
```
