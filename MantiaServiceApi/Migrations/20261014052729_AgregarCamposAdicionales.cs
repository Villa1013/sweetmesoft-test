using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MantiaServiceAPI.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCamposAdicionales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Unidad",
                table: "Tarifas",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CamposFaltantes",
                table: "Solicitudes",
                type: "TEXT",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Contacto",
                table: "Solicitudes",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "SolicitudId",
                table: "Cotizaciones",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<bool>(
                name: "EsHistorica",
                table: "Cotizaciones",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Observaciones",
                table: "Cotizaciones",
                type: "TEXT",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidaHasta",
                table: "Cotizaciones",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Solicitudes_Numero",
                table: "Solicitudes",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_Numero",
                table: "Cotizaciones",
                column: "Numero",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Solicitudes_Numero",
                table: "Solicitudes");

            migrationBuilder.DropIndex(
                name: "IX_Cotizaciones_Numero",
                table: "Cotizaciones");

            migrationBuilder.DropColumn(
                name: "Unidad",
                table: "Tarifas");

            migrationBuilder.DropColumn(
                name: "CamposFaltantes",
                table: "Solicitudes");

            migrationBuilder.DropColumn(
                name: "Contacto",
                table: "Solicitudes");

            migrationBuilder.DropColumn(
                name: "EsHistorica",
                table: "Cotizaciones");

            migrationBuilder.DropColumn(
                name: "Observaciones",
                table: "Cotizaciones");

            migrationBuilder.DropColumn(
                name: "ValidaHasta",
                table: "Cotizaciones");

            migrationBuilder.AlterColumn<int>(
                name: "SolicitudId",
                table: "Cotizaciones",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);
        }
    }
}
