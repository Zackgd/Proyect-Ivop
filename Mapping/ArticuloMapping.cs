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
            Map(x => x.stockArticulos);
            Map(x => x.listaArticulos);
            References(x => x.masterArticulo)
                .Column("idMaestroArticulo")
                .Cascade.None();

            HasOne(x => x.stockArticulos)
                .Cascade.All();
        }
    }
}
