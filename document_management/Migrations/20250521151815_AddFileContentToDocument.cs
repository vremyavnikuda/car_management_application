﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace document_management.Migrations
{
    /// <inheritdoc />
    public partial class AddFileContentToDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "FileContent",
                table: "Documents",
                type: "bytea",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileContent",
                table: "Documents");
        }
    }
}
