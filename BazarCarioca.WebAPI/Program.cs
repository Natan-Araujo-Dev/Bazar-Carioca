using BazarCarioca.WebAPI.Context;
using BazarCarioca.WebAPI.Controllers;
using BazarCarioca.WebAPI.DTOs.Mapper;
using BazarCarioca.WebAPI.Repositories;
using BazarCarioca.WebAPI.Services;
using Microsoft.EntityFrameworkCore;



var builder = WebApplication.CreateBuilder(args);

// Para funcionar fora do servidor local
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000);
});

// Add services to the container.
#region services

//JSON necess�rios para POST e PATCH
builder.Services
    .AddControllers()
    .AddNewtonsoftJson()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter()
        );
    });

/* caso v� mudar o banco de dados, adicione mais uma string como a de baixo
* (lembrando de mudar tamb�m em:
* appsettings.json > ConnectionStrings) */
string MySqlConnection = builder.Configuration.GetConnectionString("AWSRDS");

builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseMySql(MySqlConnection,
        ServerVersion.AutoDetect(MySqlConnection))
);

// Registrando o web service
builder.Services.AddScoped<IWebService, S3Service>();

// Registrando os Repositories
builder.Services.AddScoped<IShopkeeperRepository, ShopkeeperRepository>();
builder.Services.AddScoped<IStoreRepository, StoreRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IProductTypeRepository, ProductTypeRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Para o SQLController
builder.Services.AddScoped<ProductsController>();
builder.Services.AddScoped<StoresController>();

// Automapper para convers�o entre DTOs e Entidades
builder.Services.AddAutoMapper(typeof(ProductMappingProfile));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bazar Carioca");
    });
}

// Configure the HTTP request pipeline.

// Linha comentada pois HTTPS n�o est� configurado no servidor
//app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
