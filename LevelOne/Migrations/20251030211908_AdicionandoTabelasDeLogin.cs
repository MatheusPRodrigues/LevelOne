using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LevelOne.Migrations
{
    /// <inheritdoc />
    public partial class AdicionandoTabelasDeLogin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Roles",
                table: "Usuarios");

            migrationBuilder.CreateTable(
                name: "Permissoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsuariosPermissoes",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "INTEGER", nullable: false),
                    PermissaoId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosPermissoes", x => new { x.UsuarioId, x.PermissaoId });
                    table.ForeignKey(
                        name: "FK_UsuariosPermissoes_Permissoes_PermissaoId",
                        column: x => x.PermissaoId,
                        principalTable: "Permissoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuariosPermissoes_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Permissoes",
                columns: new[] { "Id", "Nome" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "Tecnico" },
                    { 3, "Cliente" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosPermissoes_PermissaoId",
                table: "UsuariosPermissoes",
                column: "PermissaoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsuariosPermissoes");

            migrationBuilder.DropTable(
                name: "Permissoes");

            migrationBuilder.AddColumn<string>(
                name: "Roles",
                table: "Usuarios",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
