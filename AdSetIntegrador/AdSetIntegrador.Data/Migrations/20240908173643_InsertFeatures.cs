using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdSetIntegrador.Data.Migrations
{
    /// <inheritdoc />
    public partial class InsertFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "OptionalFeatures",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Ar Condicionado" },
                    { 2, "Alarme" },
                    { 3, "Airbag" },
                    { 4, "Freio ABS" },
                    { 5, "Direção Hidráulica" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "OptionalFeatures",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2, 3, 4, 5 });
        }
    }
}
