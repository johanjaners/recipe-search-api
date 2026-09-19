using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeSearch.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Recipes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IngredientsRaw = table.Column<string>(type: "text", nullable: false),
                    Ingredients = table.Column<string>(type: "jsonb", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    CookTime = table.Column<string>(type: "text", nullable: false),
                    PrepTime = table.Column<string>(type: "text", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: false),
                    RecipeYield = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipes", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Recipes");
        }
    }
}
