using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;

#nullable disable
namespace KiloTaxi.EntityFramework.Migrations
{
    
    public partial class CreateTable_Driver_Vehicle : Migration {
         protected override void Up(MigrationBuilder migrationBuilder)
         {
             migrationBuilder.CreateTable(
                 name: "Driver",
                 columns: table => new
                 {
                     Id = table.Column<int>(type: "int", nullable: false)
                         .Annotation("SqlServer:Identity", "1, 1"),
                     Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                     Profile = table.Column<string>(type: "nvarchar(100)"),
                     MobilePrefix = table.Column<string>(type: "nvarchar(10)", nullable: false),
                     Phone=table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                     Email=table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                     Dob=table.Column<DateTime>(type: "datetime2", nullable: false),
                     Nrc=table.Column<string>(type: "nvarchar(50)",maxLength:50,nullable: false),
                     NrcImageFront=table.Column<string>(type: "nvarchar(max)", nullable: false),
                     NrcImageBack=table.Column<string>(type: "nvarchar(max)", nullable: false),
                     DriverLicense=table.Column<string>(type: "nvarchar(max)", nullable: false),
                     DriverImageLicenseFront=table.Column<string>(type: "nvarchar(max)", nullable: false),
                     DriverImageLicenseBack=table.Column<string>(type: "nvarchar(max)", nullable: false),
                     EmailVerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                     PhoneVerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                     Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                     Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                     Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                     Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                     State=table.Column<string>(type: "nvarchar(max)", nullable: false),
                     City=table.Column<string>(type: "nvarchar(max)", nullable: false),
                     TownShip=table.Column<string>(type: "nvarchar(max)", nullable: false),
                     DriverStatus=table.Column<string>(type: "nvarchar(max)", nullable: false),
                     KycStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                 },
                 constraints: table =>
                 {
                     table.PrimaryKey("PK_Driver", x => x.Id);
                 });
             migrationBuilder.CreateTable(
                 name: "Vehicle",
                 columns: table => new
                 {
                     Id = table.Column<int>(type: "int", nullable: false)
                         .Annotation("SqlServer:Identity", "1, 1"),
                     VehicleNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                     VehicleType = table.Column<string>(type: "nvarchar(100)",nullable: false),
                     Model = table.Column<string>(type: "nvarchar(10)", nullable: false),
                     FuelType=table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                     BusinessLicenseImage=table.Column<string>(type: "nvarchar(max)", nullable: false),
                     VehicleLicenseFront=table.Column<string>(type: "nvarchar(max)", nullable: false),
                     VehicleLicenseBack=table.Column<string>(type: "nvarchar(max)",nullable: false),
                     VehicleStatus=table.Column<string>(type: "nvarchar(max)", nullable: false),
                     DriverId=table.Column<int>(type: "int", nullable: false),
                    
                 },
                 constraints: table =>
                 {
                     table.PrimaryKey("PK_Vehicle", x => x.Id);
                     table.ForeignKey(
                         name:"FK_Vehicle_Driver_DriverId",
                         column: x => x.DriverId,
                         principalTable: "Driver",
                         principalColumn: "Id",
                         onDelete: ReferentialAction.Cascade
                         );
                 });
             migrationBuilder.CreateIndex(
                 name: "IX_Driver_Phone",
                 table: "Driver",
                 column: "Phone",
                 unique: true);
             migrationBuilder.CreateIndex(
                 name: "IX_Driver_Email",
                 table: "Driver",
                 column: "Email",
                 unique: true);
             migrationBuilder.CreateIndex(
                 name: "IX_Vehicle_DriverId",
                 table: "Vehicle",
                 column: "DriverId");
         }  

         /// <inheritdoc />
         protected override void Down(MigrationBuilder migrationBuilder)
         {
             migrationBuilder.DropTable(
                 name: "Admin");
             
             migrationBuilder.DropTable(
                 name: "Driver");
             
             migrationBuilder.DropTable(
                 name: "Vehicle");
         }
    }
    
}