using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MantiaServiceAPI.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Solicitudes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Numero = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    FechaRecepcion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Cliente = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Municipio = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Direccion = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    TelefonoContacto = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    EmailContacto = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Origen = table.Column<int>(type: "INTEGER", nullable: false),
                    Referencia = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    TextoOriginal = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                    Estado = table.Column<int>(type: "INTEGER", nullable: false),
                    Observaciones = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    FechaVisita = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FranjaHoraria = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FinDeSemana = table.Column<bool>(type: "INTEGER", nullable: false),
                    FueraDeHorario = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solicitudes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tarifas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Version = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Codigo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Concepto = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    PrecioUnitarioCOP = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    Activa = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tarifas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cotizaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Numero = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SolicitudId = table.Column<int>(type: "INTEGER", nullable: false),
                    Cliente = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Municipio = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Direccion = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    VersionTarifa = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Estado = table.Column<int>(type: "INTEGER", nullable: false),
                    Subtotal = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    IVA = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Total = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    PorcentajeIVA = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cotizaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cotizaciones_Solicitudes_SolicitudId",
                        column: x => x.SolicitudId,
                        principalTable: "Solicitudes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemsSolicitud",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SolicitudId = table.Column<int>(type: "INTEGER", nullable: false),
                    CodigoTarifa = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Concepto = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    Cantidad = table.Column<decimal>(type: "TEXT", nullable: false),
                    Observacion = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemsSolicitud", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemsSolicitud_Solicitudes_SolicitudId",
                        column: x => x.SolicitudId,
                        principalTable: "Solicitudes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LineasCotizacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CotizacionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Codigo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Concepto = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    Cantidad = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    PrecioUnitarioCOP = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    ValorLineaCOP = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineasCotizacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LineasCotizacion_Cotizaciones_CotizacionId",
                        column: x => x.CotizacionId,
                        principalTable: "Cotizaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_SolicitudId",
                table: "Cotizaciones",
                column: "SolicitudId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemsSolicitud_SolicitudId",
                table: "ItemsSolicitud",
                column: "SolicitudId");

            migrationBuilder.CreateIndex(
                name: "IX_LineasCotizacion_CotizacionId",
                table: "LineasCotizacion",
                column: "CotizacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Tarifas_Version_Codigo",
                table: "Tarifas",
                columns: new[] { "Version", "Codigo" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemsSolicitud");

            migrationBuilder.DropTable(
                name: "LineasCotizacion");

            migrationBuilder.DropTable(
                name: "Tarifas");

            migrationBuilder.DropTable(
                name: "Cotizaciones");

            migrationBuilder.DropTable(
                name: "Solicitudes");
        }
    }
}
