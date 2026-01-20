using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdsManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAutoIncrementNumberForAdvertisements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Advertisements_UserId_Number",
                table: "Advertisements");

            migrationBuilder.AlterColumn<int>(
                name: "Number",
                table: "Advertisements",
                type: "integer",
                nullable: false,
                defaultValueSql: "nextval('ads_number_seq')",
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_Advertisements_Number",
                table: "Advertisements",
                column: "Number");

            migrationBuilder.CreateIndex(
                name: "IX_Advertisements_UserId",
                table: "Advertisements",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Advertisements_Number",
                table: "Advertisements");

            migrationBuilder.DropIndex(
                name: "IX_Advertisements_UserId",
                table: "Advertisements");

            migrationBuilder.AlterColumn<int>(
                name: "Number",
                table: "Advertisements",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValueSql: "nextval('ads_number_seq')");

            migrationBuilder.CreateIndex(
                name: "IX_Advertisements_UserId_Number",
                table: "Advertisements",
                columns: new[] { "UserId", "Number" },
                unique: true);
        }
    }
}
