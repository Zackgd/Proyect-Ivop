using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Cfg;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using Proyect_InvOperativa.Repository;
using Proyect_InvOperativa.Mapping;
using Proyect_InvOperativa.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
var connectionString = builder.Configuration.GetConnectionString("MySQLConnection");
builder.Services.AddSingleton<ISessionFactory>(provider =>
{
    return Fluently.Configure()
        .Database(
            MySQLConfiguration.Standard
                .ConnectionString(connectionString) // Usar la cadena de conexión de appsettings.json
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
builder.Services.AddScoped<ArticuloService>();
builder.Services.AddScoped<MaestroArticulosService>();
builder.Services.AddScoped<OrdenCompraService>();
builder.Services.AddScoped<ProveedorService>();
builder.Services.AddScoped<VentasService>();
var apiBaseRoute = builder.Configuration.GetValue<string>("ApiBaseRoute");
builder.Services.AddControllers(); //necesario
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapGroup(apiBaseRoute!).MapControllers();
app.Run();
