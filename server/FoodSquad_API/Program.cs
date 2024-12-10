using FoodSquad_API.Data;
using FoodSquad_API.Repositories;
using FoodSquad_API.Repositories.Interfaces;
using FoodSquad_API.Services;
using FoodSquad_API.Services.Interfaces;
using FoodSquad_API.Seeders;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Filters;
using System.Text.Json.Serialization;
using FoodSquad_API;
/*
 * FoodSquad API - Startup Guide: This solution is configured to automatically handle database migrations and seeding.
 * Steps to run the project:
 * 1. Update the database connection string in appsettings.json:
 *    - Locate the "DefaultConnection" in appsettings.json and replace the value with your local SQL Server connection string.
 * 2. Run the project:
 *    - Migrations will be applied automatically.Default data will be seeded into the database during startup.
 * 3. Access the API documentation via Swagger at: https://localhost:7238/swagger
 * No manual steps like `update-database` or manual seeding are required.
 */

var builder = WebApplication.CreateBuilder(args);

// 1. Configure Database Connection
ConfigureDatabase(builder.Services, builder.Configuration);

// 2. Register Repositories (Data Access Layer)
RegisterRepositories(builder.Services);

// 3. Register Services (Business Logic Layer)
RegisterServices(builder.Services);

// 4. Add Utilities and Middleware
ConfigureUtilities(builder.Services);

// 5. Add Controllers and Views
ConfigureControllers(builder.Services);

// 6. Add CORS Policy
ConfigureCors(builder.Services);

// 7. Configure Swagger for API Documentation
ConfigureSwagger(builder.Services);

var app = builder.Build();

// 8. Configure Middleware and Run Application
ConfigureApplication(app);

#region Method Definitions

void ConfigureDatabase(IServiceCollection services, IConfiguration configuration)
{
    var connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    services.AddDbContext<MyDbContext>(options =>
        options.UseSqlServer(connectionString));
}

void RegisterRepositories(IServiceCollection services)
{
    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<ITokenRepository, TokenRepository>();
    services.AddScoped<IOrderRepository, OrderRepository>();
    services.AddScoped<IMenuItemRepository, MenuItemRepository>();
    services.AddScoped<IReviewRepository, ReviewRepository>();
}

void RegisterServices(IServiceCollection services)
{
    services.AddScoped<IAuthService, AuthService>();
    services.AddScoped<ITokenService, TokenService>();
    services.AddScoped<IUserService, UserService>();
    services.AddScoped<IMenuItemService, MenuItemService>();
    services.AddScoped<IReviewService, ReviewService>();
    services.AddScoped<IOrderService, OrderService>();
    services.AddScoped<IUserContextService, UserContextService>();
    services.AddTransient<DatabaseSeeder>();
}

void ConfigureUtilities(IServiceCollection services)
{
    services.AddAutoMapper(typeof(Program)); // Add AutoMapper for object mapping
    services.AddSingleton<JwtUtil>();       // Add JWT utilities
    services.AddScoped<JwtRequestFilter>(); // Add JWT filter middleware
    services.AddHttpContextAccessor();      // Add HttpContextAccessor for accessing HttpContext
    services.AddCustomAuthorization();// Add Custom Authorization 
}

void ConfigureControllers(IServiceCollection services)
{
    services.AddControllersWithViews(options =>
    {
        options.Filters.Add<GlobalExceptionFilter>(); // Add global exception filter
    });

    services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; // Prevent cyclic references
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; // Ignore null values
        });
}

void ConfigureCors(IServiceCollection services)
{
    services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy", policy =>
        {
            policy.WithOrigins("https://localhost:7238", "http://localhost:4200") // Add allowed origin (Swagger and frontend in this case)
                  .AllowAnyHeader() // Allow all headers
                  .AllowAnyMethod() // Allow all HTTP methods (GET, POST, etc.)
                  .AllowCredentials(); // Allow cookies and credentials
        });
    });
}

void ConfigureSwagger(IServiceCollection services)
{
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(options =>
    {
        options.EnableAnnotations(); // Enable Swagger annotations
        options.ExampleFilters();    // Add Request Example Filter

        // Add Bearer token authentication
        options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "Enter 'Bearer' [space] and then your valid token.\nExample: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'"
        });

        // Apply the Bearer token globally to all operations
        options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });
    });

    services.AddSwaggerExamplesFromAssemblyOf<Program>();
}

void ConfigureApplication(WebApplication app)
{
    try
    {
        // Configure error handling for development and production environments
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts(); // Enforce HTTPS
        }

        // Middleware execution order
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors("CorsPolicy");
        app.UseMiddleware<JwtRequestFilter>();
        app.UseMiddleware<CustomAuthenticationMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();

        // Enable Swagger
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "FoodSquad API V1");
            options.RoutePrefix = "swagger"; // Default: Serve Swagger UI at /swagger
        });

        // Configure default route
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        // Ensure the database exists and apply migrations
        InitializeDatabase(app);

        // Start the application
        app.Run();
    }
    catch (Exception ex)
    {
        // Log critical errors during startup
        Console.WriteLine($"Application failed to start: {ex.Message}");
        throw;
    }
}

void InitializeDatabase(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();

    try
    {
        Console.WriteLine("Ensuring database creation...");
        dbContext.Database.EnsureCreated(); // Creates database if not existing

        var pendingMigrations = dbContext.Database.GetPendingMigrations().ToList();

        if (pendingMigrations.Any())
        {
            Console.WriteLine("Applying pending migrations...");
            dbContext.Database.Migrate();  // Applies migrations if there are any
            Console.WriteLine("Migrations applied.");
        }
        else
        {
            Console.WriteLine("No pending migrations found.");
        }

        Console.WriteLine("Seeding the database...");
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        seeder.SeedDatabase(); // Custom seeding logic
        Console.WriteLine("Database seeding completed.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database initialization failed: {ex.Message}");
        throw;
    }
}

#endregion
