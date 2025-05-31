using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.OpenApi.Models;
using MySqlConnector;
using NHibernate;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Tool.hbm2ddl;
using Proyect_InvOperativa.Mapping;
using Proyect_InvOperativa.Repository;
using Proyect_InvOperativa.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// SWAGGER - OPEN API

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// BASE DE DATOS
var connectionString = builder.Configuration.GetConnectionString("MySQLConnection");
builder.Services.AddSingleton<ISessionFactory>(provider =>
{
    Console.WriteLine("Conectando a bd");
    return Fluently.Configure()
        .Database(
            
                MySQLConfiguration.Standard
                .ConnectionString(connectionString)
                .Dialect<MySQL8Dialect>()
                .ShowSql()
        )
        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<ArticuloMapping>())
        .ExposeConfiguration(cfg => new SchemaExport(cfg).Create(false, true))
        .BuildSessionFactory();
});

builder.Services.AddScoped<NHibernate.ISession>(provider =>
    provider.GetRequiredService<ISessionFactory>().OpenSession());

// Registro de repositorios
builder.Services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<ArticuloRepository>();
builder.Services.AddScoped<MaestroArticulosRepository>();
builder.Services.AddScoped<ProveedoresRepository>();

//Registro de Servicios
builder.Services.AddScoped<MaestroArticulosService>();
builder.Services.AddScoped<OrdenCompraService>();
builder.Services.AddScoped<VentasService>();
var apiBaseRoute = builder.Configuration.GetValue<string>("ApiBaseRoute");


builder.Services.AddControllers(); //necesario


var app = builder.Build();


// Comprobación conexión a BD
using (var scope = app.Services.CreateScope())
{
    try
    {
        var session = scope.ServiceProvider.GetRequiredService<NHibernate.ISession>();
        var result = session.CreateSQLQuery("SELECT 1").UniqueResult();
        Console.WriteLine("✅ Conexión a MySQL exitosa");
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Error de conexión: " + ex.Message);
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
