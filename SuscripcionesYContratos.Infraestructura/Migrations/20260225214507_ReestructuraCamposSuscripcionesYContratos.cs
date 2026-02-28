using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SuscripcionesYContratos.Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class ReestructuraCamposSuscripcionesYContratos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Suscripciones: quitar columna movida a Contratos
            migrationBuilder.DropColumn(
                name: "incluyeFinDeSemana",
                table: "Suscripciones");

            // Suscripciones: renombres
            migrationBuilder.RenameColumn(
                name: "precio",
                table: "Suscripciones",
                newName: "precioDia");

            migrationBuilder.RenameColumn(
                name: "cantidadEntregas",
                table: "Suscripciones",
                newName: "cantidadDias");

            // Contratos: nuevas columnas
            migrationBuilder.AddColumn<int>(
                name: "cantidadEntregas",
                table: "Contratos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "incluyeFinDeSemana",
                table: "Contratos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "precioTotal",
                table: "Contratos",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            // Mantener (o crear) relación Contratos -> Suscripciones si aplica
            migrationBuilder.CreateIndex(
                name: "IX_Contratos_suscripcionId",
                table: "Contratos",
                column: "suscripcionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contratos_Suscripciones_suscripcionId",
                table: "Contratos",
                column: "suscripcionId",
                principalTable: "Suscripciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            // Importante:
            // No se toca la tabla "Entregas" para evitar pérdida de datos.
            // La tabla permanece como fue creada en migraciones anteriores.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revertir FK e índice
            migrationBuilder.DropForeignKey(
                name: "FK_Contratos_Suscripciones_suscripcionId",
                table: "Contratos");

            migrationBuilder.DropIndex(
                name: "IX_Contratos_suscripcionId",
                table: "Contratos");

            // Revertir columnas agregadas a Contratos
            migrationBuilder.DropColumn(
                name: "cantidadEntregas",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "incluyeFinDeSemana",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "precioTotal",
                table: "Contratos");

            // Revertir renombres en Suscripciones
            migrationBuilder.RenameColumn(
                name: "precioDia",
                table: "Suscripciones",
                newName: "precio");

            migrationBuilder.RenameColumn(
                name: "cantidadDias",
                table: "Suscripciones",
                newName: "cantidadEntregas");

            // Restaurar columna que existía en Suscripciones
            migrationBuilder.AddColumn<bool>(
                name: "incluyeFinDeSemana",
                table: "Suscripciones",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            // Importante: no se recrea ni elimina "Entregas" aquí tampoco.
        }
    }
}
