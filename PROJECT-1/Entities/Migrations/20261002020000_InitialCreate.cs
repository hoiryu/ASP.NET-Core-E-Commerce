using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "countries",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_countries", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    email = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    date_of_birth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    country_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    receive_news_letters = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_countries_country_id",
                        column: x => x.country_id,
                        principalTable: "countries",
                        principalColumn: "id");
                });

            migrationBuilder.InsertData(
                table: "countries",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), "Korea" },
                    { new Guid("00000000-0000-0000-0000-000000000002"), "Thailand" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "China" },
                    { new Guid("00000000-0000-0000-0000-000000000004"), "Palestinian Territory" },
                    { new Guid("00000000-0000-0000-0000-000000000005"), "India" },
                    { new Guid("00000000-0000-0000-0000-000000000006"), "Argentina" },
                    { new Guid("00000000-0000-0000-0000-000000000007"), "Brazil" },
                    { new Guid("00000000-0000-0000-0000-000000000008"), "Canada" },
                    { new Guid("00000000-0000-0000-0000-000000000009"), "Denmark" }
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "address", "country_id", "date_of_birth", "email", "gender", "name", "receive_news_letters" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0001-000000000001"), "서울특별시", new Guid("00000000-0000-0000-0000-000000000002"), new DateTime(1989, 8, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "test1@mail.com", "Female", "김아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000002"), "경기도 안성시", new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(1990, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "test2@mail.com", "Female", "최아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000003"), "부산광역시", new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(1995, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "test3@mail.com", "Male", "박아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000004"), "경기도 수원시", new Guid("00000000-0000-0000-0000-000000000003"), new DateTime(1987, 1, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "test4@mail.com", "Male", "우아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000005"), "강원도 춘천시", new Guid("00000000-0000-0000-0000-000000000002"), new DateTime(1995, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "test5@mail.com", "Gender", "강아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000006"), "인천광역시", new Guid("00000000-0000-0000-0000-000000000004"), new DateTime(1988, 1, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "test6@mail.com", "Male", "오아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000007"), "충청북도 청주시", new Guid("00000000-0000-0000-0000-000000000003"), new DateTime(1983, 2, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "test7@mail.com", "Male", "연아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000008"), "대구광역시", new Guid("00000000-0000-0000-0000-000000000003"), new DateTime(1998, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "test8@mail.com", "Female", "유아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000009"), "전라북도 전주시", new Guid("00000000-0000-0000-0000-000000000003"), new DateTime(1990, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "test9@mail.com", "Male", "한아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000010"), "경상남도 창원시", new Guid("00000000-0000-0000-0000-000000000004"), new DateTime(1997, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "test10@mail.com", "Male", "이아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000011"), "광주광역시", new Guid("00000000-0000-0000-0000-000000000008"), new DateTime(1990, 5, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "test11@mail.com", "Female", "차아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000012"), "제주특별자치도 제주시", new Guid("00000000-0000-0000-0000-000000000008"), new DateTime(1987, 1, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "test12@mail.com", "Female", "초아무개", true }
                });

            migrationBuilder.CreateIndex(
                name: "ix_countries_name",
                table: "countries",
                column: "name",
                unique: true,
                filter: "[name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_users_country_id",
                table: "users",
                column: "country_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "countries");
        }
    }
}
