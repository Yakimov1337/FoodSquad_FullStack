using FoodSquad_API.Data;
using FoodSquad_API.Models.Entity;
using FoodSquad_API.Models.Enums;
using System.Text.Json;

namespace FoodSquad_API.Seeders
{
    public class DatabaseSeeder
    {
        private readonly MyDbContext _context;

        public DatabaseSeeder(MyDbContext context)
        {
            _context = context;
        }

        public void SeedDatabase()
        {
            if (!_context.Users.Any())
            {
                SeedUsers();
            }
            else
            {
                Console.WriteLine("Users already exist in the database, skipping user seeding.");
            }

            if (!_context.MenuItems.Any())
            {
                SeedMenuItems();
            }
            else
            {
                Console.WriteLine("Menu items already exist in the database, skipping menu item seeding.");
            }

            if (!_context.Orders.Any())
            {
                SeedOrders();
            }
            else
            {
                Console.WriteLine("Orders already exist in the database, skipping order seeding.");
            }

            if (!_context.Reviews.Any())
            {
                SeedReviews();
            }
            else
            {
                Console.WriteLine("Reviews already exist in the database, skipping review seeding.");
            }
        }

        private void SeedUsers()
        {
            var existingEmails = _context.Users.Select(u => u.Email).ToHashSet();

            var users = new List<User>
    {
        new User
        {
            Id = Guid.NewGuid(),
            Name = "John Doe",
            Email = "john.doe@example.com",
            Password = "123123", // Replace with hashed password
            Role = UserRole.Normal,
            PhoneNumber = "+359 899 78 7878",
        },
        new User
        {
            Id = Guid.NewGuid(),
            Name = "Jane Smith",
            Email = "jane.smith@example.com",
            Password = "123123",
            Role = UserRole.Normal,
            PhoneNumber = "+359 899 78 7878",
        },
        new User
        {
            Id = Guid.NewGuid(),
            Name = "Admin User",
            Email = "admin@example.com",
            Password = "123123",
            Role = UserRole.Admin,
            PhoneNumber = "+359 899 78 7878",
        },
        new User
        {
            Id = Guid.NewGuid(),
            Name = "Moderator User",
            Email = "moderator@example.com",
            Password = "123123",
            Role = UserRole.Moderator,
            PhoneNumber = "+359 899 78 7878",
        }
    };

            var newUsers = users.Where(u => !existingEmails.Contains(u.Email)).ToList();
            if (newUsers.Any())
            {
                _context.Users.AddRange(newUsers);
                _context.SaveChanges();
                Console.WriteLine("Users seeded successfully.");
            }
            else
            {
                Console.WriteLine("All users already exist. Skipping user seeding.");
            }
        }



        private void SeedMenuItems()
        {
            try
            {
                var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "menu_batch.json");

                if (!File.Exists(jsonFilePath))
                {
                    Console.WriteLine($"File not found: {jsonFilePath}");
                    return;
                }

                var jsonContent = File.ReadAllText(jsonFilePath);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var menuItemsFromJson = JsonSerializer.Deserialize<List<MenuItemJson>>(jsonContent, options);
                if (menuItemsFromJson == null || !menuItemsFromJson.Any())
                {
                    Console.WriteLine("Deserialization resulted in an empty list or null.");
                    return;
                }

                var adminUser = _context.Users.FirstOrDefault(u => u.Email == "admin@example.com");
                var moderatorUser = _context.Users.FirstOrDefault(u => u.Email == "moderator@example.com");

                if (adminUser == null || moderatorUser == null)
                {
                    Console.WriteLine("Admin or Moderator user not found. Cannot seed menu items.");
                    return;
                }

                var menuItems = new List<MenuItem>();
                foreach (var jsonItem in menuItemsFromJson)
                {
                    if (jsonItem.Creator != "adminUser" && jsonItem.Creator != "moderatorUser")
                    {
                        Console.WriteLine($"Invalid creator: {jsonItem.Creator} for menu item: {jsonItem.Title}");
                        continue;
                    }

                    var category = Enum.TryParse<MenuItemCategory>(jsonItem.Category, true, out var parsedCategory)
                        ? parsedCategory
                        : MenuItemCategory.Other;

                    var creator = jsonItem.Creator == "adminUser" ? adminUser : moderatorUser;

                    var menuItem = new MenuItem
                    {
                        Title = jsonItem.Title,
                        Description = jsonItem.Description,
                        ImageUrl = jsonItem.ImageUrl,
                        Price = jsonItem.Price,
                        DefaultItem = jsonItem.DefaultItem,
                        Category = category,
                        User = creator
                    };

                    menuItems.Add(menuItem);
                }

                if (menuItems.Any())
                {
                    _context.MenuItems.AddRange(menuItems);
                    _context.SaveChanges();
                    Console.WriteLine($"{menuItems.Count} menu items seeded successfully.");
                }
                else
                {
                    Console.WriteLine("No valid menu items found to seed.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while seeding menu items: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }




        private void SeedOrders()
        {
            var adminUser = _context.Users.FirstOrDefault(u => u.Email == "admin@example.com");
            var moderatorUser = _context.Users.FirstOrDefault(u => u.Email == "moderator@example.com");
            var menuItems = _context.MenuItems.ToList();

            if (adminUser == null || moderatorUser == null || menuItems.Count == 0)
            {
                Console.WriteLine("Required data not found. Cannot seed orders.");
                return;
            }

            var orders = new List<Order>
            {
                new Order
                {
                    User = moderatorUser,
                    CreatedOn = DateTime.UtcNow.AddDays(-5),
                    Status = OrderStatus.Completed,
                    Paid = true,
                    TotalCost = 500.0,
                    MenuItemsWithQuantity = new List<OrderMenuItem>
                    {
                        new OrderMenuItem { MenuItem = menuItems[0], Quantity = 5 },
                        new OrderMenuItem { MenuItem = menuItems[1], Quantity = 4 }
                    }
                },
                new Order
                {
                    User = adminUser,
                    CreatedOn = DateTime.UtcNow.AddDays(-3),
                    Status = OrderStatus.Pending,
                    Paid = false,
                    TotalCost = 300.0,
                    MenuItemsWithQuantity = new List<OrderMenuItem>
                    {
                        new OrderMenuItem { MenuItem = menuItems[2], Quantity = 3 },
                        new OrderMenuItem { MenuItem = menuItems[3], Quantity = 2 }
                    }
                }
            };

            _context.Orders.AddRange(orders);
            _context.SaveChanges();
            Console.WriteLine("Orders seeded successfully.");
        }

        private void SeedReviews()
        {
            var menuItems = _context.MenuItems.ToList();
            var users = _context.Users.ToList();
            var random = new Random();

            if (menuItems.Count == 0 || users.Count == 0)
            {
                Console.WriteLine("Required data not found. Cannot seed reviews.");
                return;
            }

            var comments = new[] { "Great taste!", "Could be better.", "I loved it!", "Not my favorite.", "Amazing quality!" };
            var reviews = new List<Review>();

            foreach (var menuItem in menuItems)
            {
                for (int i = 0; i < 3; i++) // Each item gets 3 reviews
                {
                    var review = new Review
                    {
                        Comment = comments[random.Next(comments.Length)],
                        Rating = random.Next(1, 6),
                        CreatedOn = DateTime.UtcNow.AddDays(-random.Next(30)),
                        User = users[random.Next(users.Count)],
                        MenuItem = menuItem
                    };

                    reviews.Add(review);
                }
            }

            _context.Reviews.AddRange(reviews);
            _context.SaveChanges();
            Console.WriteLine("Reviews seeded successfully.");
        }
    }

    public class MenuItemJson
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public double Price { get; set; }
        public bool DefaultItem { get; set; }
        public string Category { get; set; }
        public string Creator { get; set; }
    }
}
