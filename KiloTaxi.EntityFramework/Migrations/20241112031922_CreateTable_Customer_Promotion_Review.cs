using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable
namespace KiloTaxi.EntityFramework.Migrations
{
//added
    public partial class CreateTable_Customer_Promotion_Review : Migration {
             protected override void Up(MigrationBuilder migrationBuilder)
         {
             migrationBuilder.CreateTable(
                 name: "Customer",
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
                     EmailVerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                     PhoneVerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                     Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                     Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                     Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                     State=table.Column<string>(type: "nvarchar(max)", nullable: false),
                     City=table.Column<string>(type: "nvarchar(max)", nullable: false),
                     TownShip=table.Column<string>(type: "nvarchar(max)", nullable: false),
                     Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                     KycStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                 },
                 constraints: table =>
                 {
                     table.PrimaryKey("PK_Customer", x => x.Id);
                 });
             migrationBuilder.CreateTable(
                 name: "Promotion",
                 columns: table => new
                 {
                     Id = table.Column<int>(type: "int", nullable: false)
                         .Annotation("SqlServer:Identity", "1, 1"),
                     PromoCode = table.Column<string>(type: "nvarchar(100)", nullable: false),
                     ExpiredAt = table.Column<DateTime>(type: "datetime2",nullable: false),
                     FixAmount = table.Column<float>(type: "real"),
                     Percentage=table.Column<int>(type: "int"),
                     PromotionStatus=table.Column<string>(type: "nvarchar(max)", nullable: false),
                     CustomerId=table.Column<int>(type: "int", nullable: false),
                    
                 },
                 constraints: table =>
                 {
                     table.PrimaryKey("PK_Promotion", x => x.Id);
                     table.ForeignKey(
                         name:"FK_Promotion_Customer_CustomerId",
                         column: x => x.CustomerId,
                         principalTable: "Customer",
                         principalColumn: "Id",
                         onDelete: ReferentialAction.Cascade
                         );
                 });
             migrationBuilder.CreateTable(
                 name: "Review",
                 columns: table => new
                 {
                     Id = table.Column<int>(type: "int", nullable: false)
                         .Annotation("SqlServer:Identity", "1, 1"),
                     Rating = table.Column<int>(type: "int", nullable: false),
                     ReviewContent = table.Column<string>(type: "nvarchar(max)",nullable: false),
                     CustomerId=table.Column<int>(type: "int"),
                     DriverId=table.Column<int>(type: "int"),
                 },
                 constraints: table =>
                 {
                     table.PrimaryKey("PK_Review", x => x.Id);
                     table.ForeignKey(
                         name:"FK_Review_Customer_CustomerId",
                         column: x => x.CustomerId,
                         principalTable: "Customer",
                         principalColumn: "Id",
                         onDelete: ReferentialAction.Cascade);
                     table.ForeignKey(
                         name:"FK_Review_Driver_DriverId",
                         column: x => x.DriverId,
                         principalTable: "Driver",
                         principalColumn: "Id",
                         onDelete: ReferentialAction.Restrict);
                 });
             migrationBuilder.CreateIndex(
                 name: "IX_Promotion_CustomerId",
                 table: "Promotion",
                 column: "CustomerId");
             migrationBuilder.CreateIndex(
                 name: "IX_Review_CustomerId",
                 table: "Review",
                 column: "CustomerId");
             migrationBuilder.CreateIndex(
                 name: "IX_Review_DriverId",
                 table: "Review",
                 column: "DriverId");
         }  

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Customer");
             
        migrationBuilder.DropTable(
            name: "Promotion");
        
        migrationBuilder.DropTable(
            name: "Review");
    }

    }
}