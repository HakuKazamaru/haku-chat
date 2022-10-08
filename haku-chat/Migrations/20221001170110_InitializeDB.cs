using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace haku_chat.Migrations
{
    public partial class InitializeDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "chat_log",
                columns: table => new
                {
                    reg_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    name_color_id = table.Column<ushort>(type: "smallint unsigned", nullable: true),
                    message = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chat_log", x => x.reg_date);
                });

            migrationBuilder.CreateTable(
                name: "chat_login_user",
                columns: table => new
                {
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    status = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    ip_address = table.Column<string>(type: "varchar(43)", maxLength: 43, nullable: true),
                    host_name = table.Column<string>(type: "varchar(128)", nullable: true),
                    machine_name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true),
                    os_name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true),
                    user_agent = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    login_time = table.Column<DateTime>(type: "datetime", nullable: true),
                    logout_time = table.Column<DateTime>(type: "datetime", nullable: true),
                    last_update = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chat_login_user", x => x.name);
                });

            migrationBuilder.CreateTable(
                name: "chat_name_color_master",
                columns: table => new
                {
                    id = table.Column<ushort>(type: "smallint unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    code = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chat_name_color_master", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_master",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(95)", nullable: false),
                    user_id = table.Column<string>(type: "varchar(64)", nullable: false),
                    user_name = table.Column<string>(type: "varchar(64)", nullable: false),
                    password = table.Column<string>(type: "varchar(256)", nullable: true),
                    email = table.Column<string>(type: "varchar(64)", nullable: false),
                    passwordhash = table.Column<string>(type: "varchar(256)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_master", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chat_log");

            migrationBuilder.DropTable(
                name: "chat_login_user");

            migrationBuilder.DropTable(
                name: "chat_name_color_master");

            migrationBuilder.DropTable(
                name: "user_master");
        }
    }
}
