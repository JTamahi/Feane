using Microsoft.EntityFrameworkCore;
using Feane.Models;


public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }

    // Инициализация данных (Seed)
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, NameKey = "Dish_DeliciousPizza", DescriptionKey = "Dish_PizzaDescription", Price = 20, Category = "pizza", ImageUrl = "images/f1.png" },
            new Product { Id = 2, NameKey = "Dish_DeliciousBurger", DescriptionKey = "Dish_BurgerDescription", Price = 15, Category = "burger", ImageUrl = "images/f2.png" },
            new Product { Id = 3, NameKey = "Dish_DeliciousPizza", DescriptionKey = "Dish_PizzaDescription", Price = 17, Category = "pizza", ImageUrl = "images/f3.png" },
            new Product { Id = 4, NameKey = "Dish_FrenchFries", DescriptionKey = "Dish_FriesDescription", Price = 10, Category = "fries", ImageUrl = "images/f5.png" },
            new Product { Id = 5, NameKey = "Dish_DeliciousPasta", DescriptionKey = "Dish_PastaDescription", Price = 18, Category = "pasta", ImageUrl = "images/f4.png" },
            new Product { Id = 6, NameKey = "Dish_DeliciousPizza", DescriptionKey = "Dish_PizzaDescription", Price = 15, Category = "pizza", ImageUrl = "images/f6.png" },
            new Product { Id = 7, NameKey = "Dish_TastyBurger", DescriptionKey = "Dish_BurgerDescription", Price = 14, Category = "burger", ImageUrl = "images/f8.png" },
            new Product { Id = 8, NameKey = "Dish_TastyBurger", DescriptionKey = "Dish_BurgerDescription", Price = 12, Category = "burger", ImageUrl = "images/f7.png" },
            new Product { Id = 9, NameKey = "Dish_DeliciousPasta", DescriptionKey = "Dish_PastaDescription", Price = 10, Category = "pasta", ImageUrl = "images/f9.png" }
        );
    }
}