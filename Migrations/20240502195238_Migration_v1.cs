using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebApiCrawler.Migrations
{
    /// <inheritdoc />
    public partial class Migration_v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WebConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UrlPage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImgTag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Img = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StorageExtension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StorageFolder = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SearchBox = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResultType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WebStores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebStores", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Code", "Name", "StoreId" },
                values: new object[,]
                {
                    { new Guid("05af9724-9298-483a-b4f6-372b351694af"), "EK000586865", "POLAR gradski bicikl", new Guid("0a443b7c-cebb-4408-b798-a21be02040c5") },
                    { new Guid("66dd7d1e-f142-45ac-8a23-7439939b60cd"), "225463979956", "Cambridge Audio R100 FM/AM Stereo Receiver", new Guid("cc1de7be-4976-4b30-bf66-fe207218e1c8") },
                    { new Guid("997c0013-5730-4e0c-a6df-cbd708462bcd"), "155770850364", "Bluetooth Earbuds Headset", new Guid("cc1de7be-4976-4b30-bf66-fe207218e1c8") },
                    { new Guid("d6522cea-bc2c-400f-980a-736296a31c40"), "EK000525407", "Electrolux aparat za kavu", new Guid("0a443b7c-cebb-4408-b798-a21be02040c5") },
                    { new Guid("da2fe081-a3f9-463b-955b-ecc7aba0b170"), "EK000439180", "Logitech Gaming G435 Lightspeed, bežične slušalice", new Guid("0a443b7c-cebb-4408-b798-a21be02040c5") },
                    { new Guid("dd09287a-ef6b-435a-a6b9-a31bab2e041d"), "EK000467594", "TP-Link Deco E4, Mesh Wi-Fi sistem", new Guid("0a443b7c-cebb-4408-b798-a21be02040c5") }
                });

            migrationBuilder.InsertData(
                table: "WebConfigurations",
                columns: new[] { "Id", "DisplayType", "Img", "ImgTag", "ResultType", "SearchBox", "StorageExtension", "StorageFolder", "StoreId", "UrlPage" },
                values: new object[,]
                {
                    { new Guid("331d9f95-55b9-4738-a020-46781374aed7"), "WebApiCrawler.Models.ModelsInstarInformatika.SearchResultInstarInformatikaDto", "src", "//a[@class=\"fancybox\"]/img", "WebApiCrawler.Models.ModelsInstarInformatika.SearchResultInstarInformatika", "https://www.instar-informatika.hr/api/search/multi/?term=", ".jpg", "ImagesDatabase", new Guid("85f0ae0e-32c4-4110-9752-bafa7e781be1"), "https://www.instar-informatika.hr/" },
                    { new Guid("7829cb16-3ffd-41b6-a2be-f2630c0d15e4"), "WebApiCrawler.AbrakadabraModels.SearchResultAbrakadabraDto", "data-src", "//img[@data-src]", "WebApiCrawler.AbrakadabraModels.SearchResultAbrakadabra", "https://www.ekupi.hr/hr/search/autocomplete/SearchBox?term=", ".jpg", "ImagesDatabase", new Guid("0a443b7c-cebb-4408-b798-a21be02040c5"), "https://www.ekupi.hr/p/" },
                    { new Guid("7aa3d795-1db9-4e2c-8979-4908e69c3eaf"), "WebApiCrawler.AbrakadabraModels.SearchResultAbrakadabraDto", "src", "//img[@data-zoom-image]/@data-zoom-image", "WebApiCrawler.AbrakadabraModels.SearchResultAbrakadabra", "https://www.abrakadabra.com/hr-HR/search/autocomplete/SearchBox?term=", ".png", "ImagesDatabase", new Guid("eeb699b6-56e8-48ff-829d-416876d4b2de"), "https://www.abrakadabra.com/p/" },
                    { new Guid("b8bf6a33-538d-4354-9d95-bf75b33fd155"), "", "data-src", "//img[@data-src]", "", "", ".jpeg", "ImagesDatabase", new Guid("cc1de7be-4976-4b30-bf66-fe207218e1c8"), "https://www.ebay.com/itm/" },
                    { new Guid("d74c97ac-5e43-4cfa-9f4a-9c8c585d523c"), "WebApiCrawler.ModelsHGSpot.SearchResultHgSpotDto", "src", "//div[@id=\"product-gallery\"]//img", "WebApiCrawler.ModelsHGSpot.SearchResultHgSpot", "https://www.hgspot.hr/api/search-final.php?keyword=", ".jpeg", "ImagesDatabase", new Guid("f04233eb-4d2e-4c53-a5b0-9a76b5ef582f"), "https://www.hgspot.hr/" },
                    { new Guid("e8f377ef-7816-4fb9-9874-af11a5897934"), "WebApiCrawler.Models.ModelsMikronis.SearchResultMikronisDto", "src", "//div[@class=\"product-pic-inner\"]//img", "WebApiCrawler.Models.ModelsMikronis.SearchResultMikronis", "https://www.mikronis.hr/api/shopapi/getSearchSuggestion?search=", ".jpeg", "ImagesDatabase", new Guid("dfdcc0f0-6ab2-4b77-8560-2112d5dd4190"), "https://www.mikronis.hr/Proizvod/" }
                });

            migrationBuilder.InsertData(
                table: "WebStores",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("0a443b7c-cebb-4408-b798-a21be02040c5"), "eKupi" },
                    { new Guid("85f0ae0e-32c4-4110-9752-bafa7e781be1"), "Instar informatika" },
                    { new Guid("cc1de7be-4976-4b30-bf66-fe207218e1c8"), "eBay" },
                    { new Guid("dfdcc0f0-6ab2-4b77-8560-2112d5dd4190"), "Mikronis" },
                    { new Guid("eeb699b6-56e8-48ff-829d-416876d4b2de"), "Abrakadabra" },
                    { new Guid("f04233eb-4d2e-4c53-a5b0-9a76b5ef582f"), "HG Spot" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "WebConfigurations");

            migrationBuilder.DropTable(
                name: "WebStores");
        }
    }
}
