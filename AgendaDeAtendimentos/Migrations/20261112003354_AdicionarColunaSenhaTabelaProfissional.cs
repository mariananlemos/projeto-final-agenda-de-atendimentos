using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgendaDeAtendimentos.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarColunaSenhaTabelaProfissional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Senha",
                table: "Profissionals",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Senha",
                table: "Profissionals");
        }
    }
}
