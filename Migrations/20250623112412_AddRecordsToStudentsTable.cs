using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebAPI2.Migrations
{
    /// <inheritdoc />
    public partial class AddRecordsToStudentsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "Address", "DateOfBirth", "Email", "Name" },
                values: new object[,]
                {
                    { 1, "port 11, esville, ah, riv, ng, af", new DateOnly(2000, 4, 13), "yej@jey.com", "yej" },
                    { 2, "port 11, esville, ah, riv, ng, af", new DateOnly(2006, 4, 7), "uk@okey.com", "uk" },
                    { 3, "port 13, esville, ah, riv, ng, af", new DateOnly(2001, 3, 28), "josh@yeshua.com", "josh" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
