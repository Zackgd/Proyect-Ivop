using FluentNHibernate.Mapping;
using Proyect_InvOperativa.Models;

namespace Proyect_InvOperativa.Mapping
{
    public class DetalleOrdenCompraMapping : ClassMap<DetalleOrdenCompra>
    {
        public DetalleOrdenCompraMapping()
        {
            Table("DetalleCompra");
            Id(x => x.nDetalleOrdenCompra);
            Map(x => x.cantidadArticulos);
            Map(x => x.precioSubTotal);
            References(x => x.ordenCompra)
                .Column("OrdenCompra")
                .Cascade.None();
            References(x => x.articulo)
                .Column("NombreArticulo")
                .Cascade.None();
        }
    }
}

