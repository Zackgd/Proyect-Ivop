using FluentNHibernate.Mapping;
using Proyect_InvOperativa.Models;

namespace Proyect_InvOperativa.Mapping
{
    public class ProveedorArticuloMapping: ClassMap<ProveedorArticulo>
    {
        public ProveedorArticuloMapping()
        {
            Table("ProveedorArticulo");

            Id(x => x.idProveedorArticulo);
            Map(x => x.precioUnitario);
            Map(x => x.costoPedido);
            Map(x => x.tiempoEntregaDias);
            Map(x => x.fechaFinProveedorArticulo);

            References(x => x.articulo)
               .Column("articuloAsociado")
               .Cascade.None();

            References(x => x.proveedor)
                .Column("proveedor")
                .Cascade.None();
        }
    }
}
