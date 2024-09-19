using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using WebApiCrawler.AbrakadabraModels;
using WebApiCrawler.Data;
using WebApiCrawler.Models;
using WebApiCrawler.ModelsHGSpot;
using WebApiCrawler.SearchModels;
using WebApiCrawler.Models.ModelsInstarInformatika;
using WebApiCrawler.Models.ModelsMikronis;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddDbContext<CrawlerDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("CrawlerDbConnection")));
builder.Services.AddDbContext<IdentityContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("CrawlerDbConnection")));

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<IdentityContext>();
builder.Services.AddSingleton<NLog.ILogger>(_ => NLog.LogManager.GetCurrentClassLogger());

//builder.Services.AddDbContext<IdentityContext>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:5173")
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

var app = builder.Build();

// Apply migrations if needed
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CrawlerDbContext>();
    if (app.Environment.IsDevelopment())
    {
        dbContext.Database.Migrate();
    }

    var dbContext2 = scope.ServiceProvider.GetRequiredService<IdentityContext>();
    if (app.Environment.IsDevelopment())
    {
        dbContext2.Database.Migrate();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

ConfigureMapster();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseCors("AllowSpecificOrigin");


app.Run();

void ConfigureMapster()
{
    TypeAdapterConfig<SearchResultAbrakadabra, ISearchResultDto>.NewConfig()
        .Map(dest => dest.products, src => src.products.Adapt<List<IProductDto>>());

    TypeAdapterConfig<ProductAbrakadabra, IProductDto>.NewConfig()
        .Map(dest => dest.ProductName, src => src.name)
        .Map(dest => dest.PriceValue, src => src.price.value)
        .Map(dest =>dest.Link, src => src.url);

    TypeAdapterConfig<SearchResultHgSpot, ISearchResultDto>.NewConfig()
        .Map(dest => dest.products, src => src.products.Adapt<List<IProductDto>>());

    TypeAdapterConfig<ProductHgSpot, IProductDto>.NewConfig()
        .Map(dest => dest.ProductName, src => src.name)
        .Map(dest => dest.PriceValue, src => src.price_euro)
        .Map(dest =>  dest.Link, src => src.link);

    TypeAdapterConfig<SearchResultInstarInformatika, ISearchResultDto>.NewConfig()
        .Map(dest => dest.products, src => src.product.Adapt<List<IProductDto>>());

    TypeAdapterConfig<ProductInstarInformatika, IProductDto>.NewConfig()
         .Map(dest => dest.ProductName, src => src.artikl)
         .Map(dest => dest.PriceValue, src => src.price)
         .Map(dest => dest.Link, src => src.link.Substring("https://www.instar-informatika.hr/".Length));

    TypeAdapterConfig<SearchResultMikronis, ISearchResultDto>.NewConfig()
         .Map(dest => dest.products, src => src.Result.Products.Adapt<List<IProductDto>>());

    TypeAdapterConfig<ProductMicronis, IProductDto>.NewConfig()
         .Map(dest => dest.ProductName, src => src.Name)
         .Map(dest => dest.PriceValue, src => src.FinalPriceToPay)
         .Map(dest => dest.Link, src => src.GenerateReadableUrl);
}
