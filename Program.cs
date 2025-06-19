using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.OpenApi.Models;
using MySqlConnector;
using NHibernate;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Tool.hbm2ddl;
using Proyect_InvOperativa.Mapping;
using Proyect_InvOperativa.Models;
using Proyect_InvOperativa.Repository;
using Proyect_InvOperativa.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();


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
builder.Services.AddScoped<EstadoProveedoresRepository>();
builder.Services.AddScoped<MaestroArticulosRepository>();
builder.Services.AddScoped<ProveedoresRepository>();
builder.Services.AddScoped<ProveedorArticuloRepository>();
//builder.Services.AddScoped<ListaProveedoresRepository>();
builder.Services.AddScoped<OrdenCompraEstadoRepository>();
builder.Services.AddScoped<ProveedoresRepository>();
builder.Services.AddScoped<ProveedorEstadoRepository>();
builder.Services.AddScoped<VentasRepository>();
builder.Services.AddScoped<OrdenCompraRepository>();
builder.Services.AddScoped<StockArticuloRepository>();
builder.Services.AddScoped<BaseRepository<DetalleVentas>>();


//Registro de Servicios
builder.Services.AddScoped<MaestroArticulosService>();
builder.Services.AddScoped<OrdenCompraService>();
builder.Services.AddScoped<VentasService>();
builder.Services.AddScoped<ProveedorArticuloService>();
//builder.Services.AddScoped<ListaProveedoresService>();
builder.Services.AddScoped<OrdenCompraEstadoService>();
builder.Services.AddScoped<ProveedorService>();
builder.Services.AddScoped<ProveedorEstadoService>();

builder.Services.AddHostedService<Proyect_InvOperativa.Services.ControlStockPeriodoFijoService>();


var apiBaseRoute = builder.Configuration.GetValue<string>("ApiBaseRoute");


builder.Services.AddControllers(); 


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
    Exception inner = ex.InnerException;
    while (inner != null)
    {
        Console.WriteLine("Inner Exception: " + inner.Message);
        inner = inner.InnerException;
    }
}
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();

}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
