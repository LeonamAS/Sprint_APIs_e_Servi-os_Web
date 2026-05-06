using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sprint3.Migrations
{
    /// <inheritdoc />
    public partial class AtualizandoTabelaUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SenhaHash",
                table: "Usuarios",
                newName: "TipoUsuario");

            migrationBuilder.RenameColumn(
                name: "Regra",
                table: "Usuarios",
                newName: "Senha");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TipoUsuario",
                table: "Usuarios",
                newName: "SenhaHash");

            migrationBuilder.RenameColumn(
                name: "Senha",
                table: "Usuarios",
                newName: "Regra");
        }
    }
}
