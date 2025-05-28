using System.Security.Cryptography.Xml;
using FluentNHibernate.Mapping;
using Proyect_InvOperativa.Models;
namespace Proyect_InvOperativa.Mapping
{
    public class ArticuloMapping : ClassMap<Articulo>
    {
        public ArticuloMapping() 
        {
            Table("Articulos");
            Id(x => x.idArticulo).GeneratedBy.Identity();
            Map(x => x.nombreArticulo);
            Map(x => x.descripcion);
            References(x => x.listaArticulos)
                .Column("listaDeArticulos")
                .Cascade.None();
            References(x => x.masterArticulo)
                .Column("idMaestroArticulo")
                .Cascade.None();

            HasMany(x => x.stockArticulos)
    .KeyColumn("idArticulo") // FK en la tabla StockArticulos que apunta a Articulo
    .Cascade.All()
    .Inverse(); 

        }
    }
}
