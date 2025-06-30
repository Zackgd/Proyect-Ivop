using FluentNHibernate.Mapping;
using Proyect_InvOperativa.Models;

namespace Proyect_InvOperativa.Mapping
{
    public class VentaDetalleMapping : ClassMap<VentaDetalle>
    {
        public VentaDetalleMapping()
        {
            Table("VentaDetalle");

            Id(x => x.nDetalleVenta).GeneratedBy.Identity();
            Map(x => x.subTotalVenta);
            Map(x => x.cantidad);

            References(x => x.venta).Column("NVENTA");
            References(x => x.articulo).Column("idArticulo");
        }
    }
}
