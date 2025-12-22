using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Feane.Migrations.AppDb
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DescriptionKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "DescriptionKey", "ImageUrl", "NameKey", "Price" },
                values: new object[,]
                {
                    { 1, "pizza", "Dish_PizzaDescription", "images/f1.png", "Dish_DeliciousPizza", 20m },
                    { 2, "burger", "Dish_BurgerDescription", "images/f2.png", "Dish_DeliciousBurger", 15m },
                    { 3, "pizza", "Dish_PizzaDescription", "images/f3.png", "Dish_DeliciousPizza", 17m },
                    { 4, "fries", "Dish_FriesDescription", "imagesf5.png", "Dish_FrenchFries", 10m },
                    { 5, "pasta", "Dish_PastaDescription", "images/f4.png", "Dish_DeliciousPasta", 18m },
                    { 6, "pizza", "Dish_PizzaDescription", "images/f6.png", "Dish_DeliciousPizza", 15m },
                    { 7, "burger", "Dish_BurgerDescription", "images/f8.png", "Dish_TastyBurger", 14m },
                    { 8, "burger", "Dish_BurgerDescription", "images/f7.png", "Dish_TastyBurger", 12m },
                    { 9, "pasta", "Dish_PastaDescription", "images/f9.png", "Dish_DeliciousPasta", 10m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
