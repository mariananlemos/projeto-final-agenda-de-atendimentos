using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgendaDeAtendimentos.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarSenhasHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Senha",
                table: "Profissionals",
                newName: "SenhaSalt");

            migrationBuilder.AddColumn<string>(
                name: "SenhaHash",
                table: "Profissionals",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SenhaHash",
                table: "Profissionals");

            migrationBuilder.RenameColumn(
                name: "SenhaSalt",
                table: "Profissionals",
                newName: "Senha");
        }
    }
}
