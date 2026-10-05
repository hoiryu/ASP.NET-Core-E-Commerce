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
                    { new Guid("00000000-0000-0000-0000-000000000009"), "Denmark" },
                    { new Guid("00000000-0000-0000-0000-000000000010"), "Japan" },
                    { new Guid("00000000-0000-0000-0000-000000000011"), "Vietnam" },
                    { new Guid("00000000-0000-0000-0000-000000000012"), "Philippines" },
                    { new Guid("00000000-0000-0000-0000-000000000013"), "Indonesia" },
                    { new Guid("00000000-0000-0000-0000-000000000014"), "Malaysia" },
                    { new Guid("00000000-0000-0000-0000-000000000015"), "Singapore" },
                    { new Guid("00000000-0000-0000-0000-000000000016"), "Australia" },
                    { new Guid("00000000-0000-0000-0000-000000000017"), "New Zealand" },
                    { new Guid("00000000-0000-0000-0000-000000000018"), "United States" },
                    { new Guid("00000000-0000-0000-0000-000000000019"), "Mexico" },
                    { new Guid("00000000-0000-0000-0000-000000000020"), "United Kingdom" },
                    { new Guid("00000000-0000-0000-0000-000000000021"), "France" },
                    { new Guid("00000000-0000-0000-0000-000000000022"), "Germany" },
                    { new Guid("00000000-0000-0000-0000-000000000023"), "Italy" },
                    { new Guid("00000000-0000-0000-0000-000000000024"), "Spain" },
                    { new Guid("00000000-0000-0000-0000-000000000025"), "Netherlands" },
                    { new Guid("00000000-0000-0000-0000-000000000026"), "Sweden" },
                    { new Guid("00000000-0000-0000-0000-000000000027"), "Norway" },
                    { new Guid("00000000-0000-0000-0000-000000000028"), "Finland" },
                    { new Guid("00000000-0000-0000-0000-000000000029"), "Turkey" },
                    { new Guid("00000000-0000-0000-0000-000000000030"), "Egypt" }
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
                    { new Guid("00000000-0000-0000-0001-000000000012"), "제주특별자치도 제주시", new Guid("00000000-0000-0000-0000-000000000008"), new DateTime(1987, 1, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "test12@mail.com", "Female", "초아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000013"), "제주특별자치도 서귀포시", new Guid("00000000-0000-0000-0000-000000000005"), new DateTime(2000, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "test13@mail.com", "Male", "차아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000014"), "울산광역시", new Guid("00000000-0000-0000-0000-000000000003"), new DateTime(1997, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "test14@mail.com", "Male", "도아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000015"), "충청남도 천안시", new Guid("00000000-0000-0000-0000-000000000023"), new DateTime(1999, 1, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "test15@mail.com", "Female", "배아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000016"), "제주특별자치도 제주시", new Guid("00000000-0000-0000-0000-000000000006"), new DateTime(1994, 10, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "test16@mail.com", "Female", "김아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000017"), "인천광역시", new Guid("00000000-0000-0000-0000-000000000013"), new DateTime(1988, 3, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "test17@mail.com", "Male", "채아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000018"), "인천광역시", new Guid("00000000-0000-0000-0000-000000000018"), new DateTime(1991, 10, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "test18@mail.com", "Female", "강아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000019"), "경기도 용인시", new Guid("00000000-0000-0000-0000-000000000020"), new DateTime(1992, 2, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "test19@mail.com", "Female", "우아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000020"), "대구광역시", new Guid("00000000-0000-0000-0000-000000000010"), new DateTime(2002, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "test20@mail.com", "Male", "봉아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000021"), "대전광역시", new Guid("00000000-0000-0000-0000-000000000012"), new DateTime(1983, 7, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "test21@mail.com", "Female", "설아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000022"), "경상북도 포항시", new Guid("00000000-0000-0000-0000-000000000020"), new DateTime(1986, 11, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "test22@mail.com", "Female", "황아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000023"), "경상북도 포항시", new Guid("00000000-0000-0000-0000-000000000030"), new DateTime(1987, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "test23@mail.com", "Male", "현아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000024"), "경기도 고양시", new Guid("00000000-0000-0000-0000-000000000002"), new DateTime(1987, 11, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "test24@mail.com", "Male", "윤아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000025"), "경기도 안성시", new Guid("00000000-0000-0000-0000-000000000016"), new DateTime(1982, 4, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "test25@mail.com", "Male", "진아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000026"), "충청남도 천안시", new Guid("00000000-0000-0000-0000-000000000024"), new DateTime(2000, 8, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "test26@mail.com", "Male", "곽아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000027"), "충청북도 청주시", new Guid("00000000-0000-0000-0000-000000000005"), new DateTime(1998, 7, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "test27@mail.com", "Male", "염아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000028"), "경기도 파주시", new Guid("00000000-0000-0000-0000-000000000022"), new DateTime(1981, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "test28@mail.com", "Male", "편아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000029"), "서울특별시", new Guid("00000000-0000-0000-0000-000000000018"), new DateTime(1992, 7, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "test29@mail.com", "Female", "마아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000030"), "인천광역시", new Guid("00000000-0000-0000-0000-000000000011"), new DateTime(1983, 11, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "test30@mail.com", "Female", "성아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000031"), "대전광역시", new Guid("00000000-0000-0000-0000-000000000025"), new DateTime(1985, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "test31@mail.com", "Other", "곽아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000032"), "광주광역시", new Guid("00000000-0000-0000-0000-000000000007"), new DateTime(1983, 11, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "test32@mail.com", "Female", "용아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000033"), "경기도 고양시", new Guid("00000000-0000-0000-0000-000000000020"), new DateTime(1985, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "test33@mail.com", "Other", "왕아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000034"), "전라북도 전주시", new Guid("00000000-0000-0000-0000-000000000008"), new DateTime(1983, 6, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "test34@mail.com", "Male", "민아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000035"), "광주광역시", new Guid("00000000-0000-0000-0000-000000000018"), new DateTime(1982, 12, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "test35@mail.com", "Other", "장아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000036"), "경기도 파주시", new Guid("00000000-0000-0000-0000-000000000020"), new DateTime(1995, 9, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "test36@mail.com", "Female", "곽아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000037"), "경상남도 창원시", new Guid("00000000-0000-0000-0000-000000000013"), new DateTime(1997, 12, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "test37@mail.com", "Female", "배아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000038"), "경기도 고양시", new Guid("00000000-0000-0000-0000-000000000003"), new DateTime(1994, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "test38@mail.com", "Male", "황아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000039"), "경상북도 포항시", new Guid("00000000-0000-0000-0000-000000000023"), new DateTime(1997, 4, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "test39@mail.com", "Male", "유아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000040"), "경기도 수원시", new Guid("00000000-0000-0000-0000-000000000008"), new DateTime(1982, 1, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "test40@mail.com", "Male", "엄아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000041"), "강원도 강릉시", new Guid("00000000-0000-0000-0000-000000000008"), new DateTime(1986, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "test41@mail.com", "Female", "제아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000042"), "경기도 파주시", new Guid("00000000-0000-0000-0000-000000000014"), new DateTime(1986, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "test42@mail.com", "Male", "봉아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000043"), "제주특별자치도 서귀포시", new Guid("00000000-0000-0000-0000-000000000013"), new DateTime(1981, 11, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "test43@mail.com", "Male", "예아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000044"), "경기도 파주시", new Guid("00000000-0000-0000-0000-000000000005"), new DateTime(1983, 4, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "test44@mail.com", "Female", "손아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000045"), "충청남도 천안시", new Guid("00000000-0000-0000-0000-000000000028"), new DateTime(1994, 4, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "test45@mail.com", "Male", "임아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000046"), "세종특별자치시", new Guid("00000000-0000-0000-0000-000000000030"), new DateTime(2000, 9, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "test46@mail.com", "Other", "이아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000047"), "경기도 안성시", new Guid("00000000-0000-0000-0000-000000000006"), new DateTime(1995, 8, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "test47@mail.com", "Other", "염아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000048"), "제주특별자치도 서귀포시", new Guid("00000000-0000-0000-0000-000000000023"), new DateTime(1992, 5, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "test48@mail.com", "Male", "설아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000049"), "울산광역시", new Guid("00000000-0000-0000-0000-000000000010"), new DateTime(1997, 11, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "test49@mail.com", "Male", "위아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000050"), "부산광역시", new Guid("00000000-0000-0000-0000-000000000002"), new DateTime(1998, 12, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "test50@mail.com", "Female", "윤아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000051"), "대전광역시", new Guid("00000000-0000-0000-0000-000000000003"), new DateTime(1996, 9, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "test51@mail.com", "Other", "윤아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000052"), "전라북도 전주시", new Guid("00000000-0000-0000-0000-000000000029"), new DateTime(1982, 11, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "test52@mail.com", "Male", "심아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000053"), "전라북도 전주시", new Guid("00000000-0000-0000-0000-000000000019"), new DateTime(1999, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "test53@mail.com", "Male", "한아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000054"), "광주광역시", new Guid("00000000-0000-0000-0000-000000000013"), new DateTime(1988, 4, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "test54@mail.com", "Male", "진아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000055"), "전라북도 전주시", new Guid("00000000-0000-0000-0000-000000000020"), new DateTime(1989, 8, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "test55@mail.com", "Male", "임아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000056"), "경기도 용인시", new Guid("00000000-0000-0000-0000-000000000030"), new DateTime(1982, 9, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "test56@mail.com", "Male", "명아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000057"), "제주특별자치도 제주시", new Guid("00000000-0000-0000-0000-000000000018"), new DateTime(1987, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "test57@mail.com", "Male", "홍아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000058"), "경상남도 창원시", new Guid("00000000-0000-0000-0000-000000000010"), new DateTime(2000, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "test58@mail.com", "Female", "사아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000059"), "경기도 수원시", new Guid("00000000-0000-0000-0000-000000000005"), new DateTime(1984, 5, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "test59@mail.com", "Female", "신아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000060"), "경기도 수원시", new Guid("00000000-0000-0000-0000-000000000028"), new DateTime(1986, 12, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "test60@mail.com", "Female", "백아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000061"), "부산광역시", new Guid("00000000-0000-0000-0000-000000000009"), new DateTime(1988, 1, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "test61@mail.com", "Male", "용아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000062"), "제주특별자치도 제주시", new Guid("00000000-0000-0000-0000-000000000018"), new DateTime(1984, 11, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "test62@mail.com", "Female", "홍아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000063"), "부산광역시", new Guid("00000000-0000-0000-0000-000000000018"), new DateTime(1980, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "test63@mail.com", "Other", "보아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000064"), "경기도 용인시", new Guid("00000000-0000-0000-0000-000000000010"), new DateTime(1998, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "test64@mail.com", "Male", "소아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000065"), "경기도 용인시", new Guid("00000000-0000-0000-0000-000000000004"), new DateTime(1981, 6, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "test65@mail.com", "Male", "황아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000066"), "대전광역시", new Guid("00000000-0000-0000-0000-000000000008"), new DateTime(1993, 10, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "test66@mail.com", "Other", "전아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000067"), "경기도 파주시", new Guid("00000000-0000-0000-0000-000000000011"), new DateTime(1985, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "test67@mail.com", "Female", "문아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000068"), "강원도 강릉시", new Guid("00000000-0000-0000-0000-000000000002"), new DateTime(1987, 5, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "test68@mail.com", "Male", "신아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000069"), "울산광역시", new Guid("00000000-0000-0000-0000-000000000022"), new DateTime(1994, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "test69@mail.com", "Male", "남아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000070"), "경기도 안성시", new Guid("00000000-0000-0000-0000-000000000017"), new DateTime(1988, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "test70@mail.com", "Male", "차아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000071"), "대전광역시", new Guid("00000000-0000-0000-0000-000000000009"), new DateTime(1997, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "test71@mail.com", "Female", "권아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000072"), "경기도 고양시", new Guid("00000000-0000-0000-0000-000000000024"), new DateTime(1988, 1, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "test72@mail.com", "Male", "탁아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000073"), "제주특별자치도 제주시", new Guid("00000000-0000-0000-0000-000000000002"), new DateTime(1996, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "test73@mail.com", "Male", "제아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000074"), "대구광역시", new Guid("00000000-0000-0000-0000-000000000014"), new DateTime(1996, 9, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "test74@mail.com", "Male", "사아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000075"), "경기도 성남시", new Guid("00000000-0000-0000-0000-000000000024"), new DateTime(1990, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "test75@mail.com", "Female", "봉아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000076"), "광주광역시", new Guid("00000000-0000-0000-0000-000000000018"), new DateTime(2001, 7, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "test76@mail.com", "Female", "염아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000077"), "경기도 안성시", new Guid("00000000-0000-0000-0000-000000000010"), new DateTime(2001, 7, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "test77@mail.com", "Female", "문아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000078"), "전라남도 목포시", new Guid("00000000-0000-0000-0000-000000000019"), new DateTime(1980, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "test78@mail.com", "Male", "백아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000079"), "제주특별자치도 서귀포시", new Guid("00000000-0000-0000-0000-000000000016"), new DateTime(1994, 8, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "test79@mail.com", "Male", "부아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000080"), "대구광역시", new Guid("00000000-0000-0000-0000-000000000011"), new DateTime(1982, 5, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "test80@mail.com", "Female", "봉아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000081"), "서울특별시", new Guid("00000000-0000-0000-0000-000000000005"), new DateTime(1987, 11, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "test81@mail.com", "Female", "유아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000082"), "경상북도 포항시", new Guid("00000000-0000-0000-0000-000000000029"), new DateTime(1995, 10, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "test82@mail.com", "Female", "임아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000083"), "경상북도 포항시", new Guid("00000000-0000-0000-0000-000000000005"), new DateTime(2002, 12, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "test83@mail.com", "Male", "표아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000084"), "충청북도 청주시", new Guid("00000000-0000-0000-0000-000000000023"), new DateTime(1983, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "test84@mail.com", "Female", "문아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000085"), "강원도 춘천시", new Guid("00000000-0000-0000-0000-000000000026"), new DateTime(1997, 4, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "test85@mail.com", "Female", "황아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000086"), "충청북도 청주시", new Guid("00000000-0000-0000-0000-000000000024"), new DateTime(1997, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "test86@mail.com", "Female", "석아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000087"), "세종특별자치시", new Guid("00000000-0000-0000-0000-000000000025"), new DateTime(1997, 8, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "test87@mail.com", "Female", "길아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000088"), "대구광역시", new Guid("00000000-0000-0000-0000-000000000015"), new DateTime(1988, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "test88@mail.com", "Male", "편아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000089"), "광주광역시", new Guid("00000000-0000-0000-0000-000000000003"), new DateTime(1987, 5, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "test89@mail.com", "Female", "진아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000090"), "경기도 고양시", new Guid("00000000-0000-0000-0000-000000000014"), new DateTime(1992, 12, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "test90@mail.com", "Male", "허아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000091"), "전라북도 전주시", new Guid("00000000-0000-0000-0000-000000000025"), new DateTime(1993, 1, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "test91@mail.com", "Male", "추아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000092"), "경기도 성남시", new Guid("00000000-0000-0000-0000-000000000012"), new DateTime(1980, 10, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "test92@mail.com", "Male", "연아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000093"), "세종특별자치시", new Guid("00000000-0000-0000-0000-000000000029"), new DateTime(1993, 9, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "test93@mail.com", "Female", "옥아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000094"), "경상남도 창원시", new Guid("00000000-0000-0000-0000-000000000022"), new DateTime(1988, 7, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "test94@mail.com", "Male", "최아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000095"), "경기도 안성시", new Guid("00000000-0000-0000-0000-000000000030"), new DateTime(1985, 8, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "test95@mail.com", "Female", "은아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000096"), "강원도 춘천시", new Guid("00000000-0000-0000-0000-000000000028"), new DateTime(2001, 1, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "test96@mail.com", "Male", "예아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000097"), "경기도 안성시", new Guid("00000000-0000-0000-0000-000000000011"), new DateTime(1988, 7, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "test97@mail.com", "Female", "허아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000098"), "충청남도 천안시", new Guid("00000000-0000-0000-0000-000000000024"), new DateTime(1993, 5, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "test98@mail.com", "Female", "한아무개", true },
                    { new Guid("00000000-0000-0000-0001-000000000099"), "부산광역시", new Guid("00000000-0000-0000-0000-000000000021"), new DateTime(1991, 4, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "test99@mail.com", "Female", "장아무개", false },
                    { new Guid("00000000-0000-0000-0001-000000000100"), "광주광역시", new Guid("00000000-0000-0000-0000-000000000008"), new DateTime(1987, 4, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "test100@mail.com", "Female", "박아무개", true }
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
