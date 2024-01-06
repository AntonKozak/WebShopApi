using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class CountryAndCity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CactusPhotos_Cacti_CactusId",
                table: "CactusPhotos");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersPhotos_Users_UserId",
                table: "UsersPhotos");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UsersPhotos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsMain",
                table: "UsersPhotos",
                type: "INTEGER",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsMain",
                table: "CactusPhotos",
                type: "INTEGER",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CactusId",
                table: "CactusPhotos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CactusPhotos_Cacti_CactusId",
                table: "CactusPhotos",
                column: "CactusId",
                principalTable: "Cacti",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersPhotos_Users_UserId",
                table: "UsersPhotos",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CactusPhotos_Cacti_CactusId",
                table: "CactusPhotos");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersPhotos_Users_UserId",
                table: "UsersPhotos");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UsersPhotos",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<bool>(
                name: "IsMain",
                table: "UsersPhotos",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<bool>(
                name: "IsMain",
                table: "CactusPhotos",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "CactusId",
                table: "CactusPhotos",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_CactusPhotos_Cacti_CactusId",
                table: "CactusPhotos",
                column: "CactusId",
                principalTable: "Cacti",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersPhotos_Users_UserId",
                table: "UsersPhotos",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
