using Microsoft.EntityFrameworkCore.Migrations;

namespace RoomBookingApi.Migrations
{
    public partial class InitPostgres : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Tes ALTER TABLE classiques
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Rooms",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            // Exemple de conversion spéciale pour DateTime
            migrationBuilder.Sql(
                "ALTER TABLE \"Reservations\" ALTER COLUMN \"Date\" TYPE timestamp with time zone USING \"Date\"::timestamp with time zone;");

            // Exemple de conversion spéciale pour bool
            migrationBuilder.Sql(
                "ALTER TABLE \"AspNetUsers\" ALTER COLUMN \"PhoneNumberConfirmed\" TYPE boolean USING \"PhoneNumberConfirmed\"::boolean;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Inverser les changements de la méthode Up
            // Exemples :
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Rooms",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.Sql(
                "ALTER TABLE \"Reservations\" ALTER COLUMN \"Date\" TYPE TEXT USING \"Date\"::text;");

            migrationBuilder.Sql(
                "ALTER TABLE \"AspNetUsers\" ALTER COLUMN \"PhoneNumberConfirmed\" TYPE INTEGER USING CASE WHEN \"PhoneNumberConfirmed\" THEN 1 ELSE 0 END;");
        }
    }
}
