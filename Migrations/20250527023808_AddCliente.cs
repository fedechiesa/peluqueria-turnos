using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurnosPeluqueria.Migrations
{
    /// <inheritdoc />
    public partial class AddCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cliente",
                table: "Turnos");

            migrationBuilder.RenameColumn(
                name: "FechaHora",
                table: "Turnos",
                newName: "Fecha");

            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "Turnos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_ClienteId",
                table: "Turnos",
                column: "ClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Turnos_Clientes_ClienteId",
                table: "Turnos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Turnos_Clientes_ClienteId",
                table: "Turnos");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Turnos_ClienteId",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Turnos");

            migrationBuilder.RenameColumn(
                name: "Fecha",
                table: "Turnos",
                newName: "FechaHora");

            migrationBuilder.AddColumn<string>(
                name: "Cliente",
                table: "Turnos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
