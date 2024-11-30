using FoodSquad_API.Data;
using FoodSquad_API.Repositories;
using FoodSquad_API.Repositories.Interfaces;
using FoodSquad_API.Services;
using FoodSquad_API.Services.Interfaces;
using FoodSquad_API.Seeders;
using FoodSquad_API.Middleware;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Configure the database connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register repositories (data access layer)
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IMenuItemRepository, MenuItemRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

// Register services (business logic layer)
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMenuItemService, MenuItemService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IUserContextService, UserContextService>();

// Add AutoMapper for object mapping
builder.Services.AddAutoMapper(typeof(Program));

// Add JWT utilities
builder.Services.AddSingleton<JwtUtil>();
builder.Services.AddScoped<JwtRequestFilter>();

// Add HttpContextAccessor to access HttpContext in services
builder.Services.AddHttpContextAccessor();

// Register custom authentication and authorization
builder.Services.AddCustomAuthentication(builder.Configuration);
builder.Services.AddCustomAuthorization();

// Add controllers and views
builder.Services.AddControllersWithViews(options =>
{
    // Add global exception filter
    options.Filters.Add<GlobalExceptionFilter>();
});

// Register the CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("https://localhost:7238") // Add allowed origin (Swagger in this case)
              .AllowAnyHeader() // Allow all headers
              .AllowAnyMethod() // Allow all HTTP methods (GET, POST, etc.)
              .AllowCredentials(); // Allow cookies and credentials
    });
});

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
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
    // Add operation filter to include examples
    options.OperationFilter<SwaggerDefaultValues>();
});

builder.Services.AddTransient<DatabaseSeeder>();

var app = builder.Build();

// Logging setup for critical error handling during startup
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
    app.UseMiddleware<JwtRequestFilter>();
    app.UseMiddleware<CustomExceptionMiddleware>(); // Global error handling middleware
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
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();

        // Ensure database is created
        Console.WriteLine("Creating database (if it doesn't exist)...");
        dbContext.Database.EnsureCreated();
        Console.WriteLine("Database created.");

        // Apply migrations
        Console.WriteLine("Applying migrations...");
        dbContext.Database.Migrate();
        Console.WriteLine("Migrations applied successfully.");

        // Run the database seeder
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        Console.WriteLine("Seeding the database...");
        seeder.SeedDatabase();
        Console.WriteLine("Database seeding completed.");
    }


    // Start the application
    app.Run();
}
catch (Exception ex)
{
    // Log critical errors during startup
    Console.WriteLine($"Application failed to start: {ex.Message}");
    throw;
}
