using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SuscripcionesYContratos.Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class InitialStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    eventname = table.Column<string>(type: "text", nullable: false),
                    OccurredOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Type = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    Payload = table.Column<string>(type: "text", nullable: false),
                    ProcessedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Error = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Suscripciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "text", nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: false),
                    cantidadDias = table.Column<int>(type: "integer", nullable: false),
                    precioDia = table.Column<decimal>(type: "numeric", nullable: false),
                    updateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suscripciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contratos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    pacienteId = table.Column<Guid>(type: "uuid", nullable: false),
                    suscripcionId = table.Column<Guid>(type: "uuid", nullable: false),
                    planId = table.Column<Guid>(type: "uuid", nullable: false),
                    hora = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    inicio = table.Column<DateOnly>(type: "date", nullable: false),
                    fin = table.Column<DateOnly>(type: "date", nullable: false),
                    incluyeFinDeSemana = table.Column<bool>(type: "boolean", nullable: false),
                    cantidadEntregas = table.Column<int>(type: "integer", nullable: false),
                    precioTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    politicaCancelacionDias = table.Column<int>(type: "integer", nullable: false),
                    updateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contratos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contratos_Suscripciones_suscripcionId",
                        column: x => x.suscripcionId,
                        principalTable: "Suscripciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CalendarioEntrega",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    contratoId = table.Column<Guid>(type: "uuid", nullable: false),
                    fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    hora = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    updateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarioEntrega", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalendarioEntrega_Contratos_contratoId",
                        column: x => x.contratoId,
                        principalTable: "Contratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarioEntrega_contratoId",
                table: "CalendarioEntrega",
                column: "contratoId");

            migrationBuilder.CreateIndex(
                name: "IX_Contratos_suscripcionId",
                table: "Contratos",
                column: "suscripcionId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_ProcessedOnUtc",
                table: "OutboxMessages",
                column: "ProcessedOnUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalendarioEntrega");

            migrationBuilder.DropTable(
                name: "OutboxMessages");

            migrationBuilder.DropTable(
                name: "Contratos");

            migrationBuilder.DropTable(
                name: "Suscripciones");
        }
    }
}
