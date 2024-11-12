using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KiloTaxi.EntityFramework.Migrations
{
    [DbContext(typeof(DbKiloTaxiContext))]
    [Migration("20241111083012_CreateTable_Driver_Vehicle")]

    partial class CreateTable_Driver_Vehicle{
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
        #pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("KiloTaxi.EntityFramework.EntityModel.Driver", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));
                
                b.Property<string>("Name")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");
                
                b.Property<string>("Profile")
                    .HasColumnType("nvarchar(max)");
                
                b.Property<string>("MobilePrefix")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");
                
                b.Property<string>("Phone")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");
                
                b.Property<string>("Email")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");
                
                b.Property<DateTime>("Dob")
                    .HasColumnType("datetime2");
                
                b.Property<string>("Nrc")
                    .HasColumnType("nvarchar(max)");
                
                b.Property<string>("NrcImageFront")
                    .HasColumnType("nvarchar(max)");
                
                b.Property<string>("NrcImageBack")
                    .HasColumnType("nvarchar(max)");
                
                b.Property<string>("DriverLicense")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");
                
                b.Property<string>("DriverImageLicenseFront")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");
                
                b.Property<string>("DriverImageLicenseBack")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");
                
                b.Property<DateTime>("EmailVerifiedAt")
                    .HasColumnType("datetime2");
                
                b.Property<DateTime>("PhoneVerifiedAt")
                    .HasColumnType("datetime2");
                
                b.Property<string>("Password")
                    .HasColumnType("datetime2");
         

                b.HasKey("Id");
                

                b.ToTable("Cities");
            });
        }
    }
}