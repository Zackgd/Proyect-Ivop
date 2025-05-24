using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Cfg;
using NHibernate;
using System;
using NHibernate.Tool.hbm2ddl;
using Proyect_InvOperativa.Models;
using Proyect_InvOperativa.Repository;
using Proyect_InvOperativa.Mapping;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();


builder.Services.AddSingleton(provider =>
{
    return Fluently.Configure()
        .Database(
            MySQLConfiguration.Standard
                .ConnectionString("Server=localhost;Database=nombre_db;User=root;Password=tu_password;")
                .ShowSql()
        )
        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<ArticuloMapping>())
        .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(false, true))
        .BuildSessionFactory();
});
var sessionFactory = Fluently.Configure()
    .Database(MsSqlConfiguration.MsSql2012
        .ConnectionString(builder.Configuration.GetConnectionString("DefaultConnection"))
        .ShowSql()
    )
    .Mappings(m => m.FluentMappings.AddFromAssembly(typeof(Program).Assembly))
    .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(false, true))
    .BuildSessionFactory();

builder.Services.AddSingleton<ISessionFactory>(sessionFactory);
builder.Services.AddScoped<NHibernate.ISession>(provider =>
    provider.GetRequiredService<ISessionFactory>().OpenSession());

// Registro de repositorios
builder.Services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<ArticuloRepository>();

builder.Services.AddScoped<ArticuloRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
