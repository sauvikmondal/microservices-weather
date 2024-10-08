﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudWeather.Temparature.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "temparature",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TempHighF = table.Column<decimal>(type: "numeric", nullable: false),
                    TempLowF = table.Column<decimal>(type: "numeric", nullable: false),
                    ZipCode = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_temparature", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "temparature");
        }
    }
}
