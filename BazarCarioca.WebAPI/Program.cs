using BazarCarioca.WebAPI.Context;
using BazarCarioca.WebAPI.Repositories;
using BazarCarioca.WebAPI.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#region services


builder.Services.AddControllers();

builder.Services
    .AddControllers()
    .AddNewtonsoftJson();

/* caso v� mudar o banco de dados, adicione mais uma string como a de baixo
* (lembrando de mudar tamb�m em:
* appsettings.json > ConnectionStrings) */
string MySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseMySql(MySqlConnection,
        ServerVersion.AutoDetect(MySqlConnection))
);

// Registrando o servi�o do AWS
builder.Services.AddScoped<IS3Service, S3Service>();

// Registrando os Repositories
builder.Services.AddScoped<IShopkeeperRepository, ShopkeeperRepository>();
builder.Services.AddScoped<IStoreRepository, StoreRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IProductTypeRepository, ProductTypeRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Necess�rio para o Post
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
        
        // caso d� o poss�vel erro de c�clos
        //opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });


//builder.Services.AddControllers().AddNewtonsoftJson();

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

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
