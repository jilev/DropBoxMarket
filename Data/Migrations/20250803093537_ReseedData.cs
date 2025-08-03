using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DropBoxMarket.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReseedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Price" },
                values: new object[] { "Latest model with advanced features", 699.99m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "ImageUrl", "Price", "Title" },
                values: new object[] { "High power vacuum cleaner", "/images/vacuum.jpg", 199.99m, "Vacuum Cleaner" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "ImageUrl", "Price", "Title" },
                values: new object[] { "Cotton T-Shirt for everyday use", "/images/tshirt.jpg", 19.99m, "T-Shirt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Price" },
                values: new object[] { "Latest model smartphone with 5G", 899.99m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "ImageUrl", "Price", "Title" },
                values: new object[] { "High-quality coffee maker for your home", "/images/coffeemaker.jpg", 120.50m, "Coffee Maker" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "ImageUrl", "Price", "Title" },
                values: new object[] { "Stylish winter jacket", "/images/jacket.jpg", 79.99m, "Men's Jacket" });
        }
    }
}
