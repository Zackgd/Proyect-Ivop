using FluentNHibernate.Mapping;
using Proyect_InvOperativa.Models;

namespace Proyect_InvOperativa.Mapping
{
    public class OrdenCompraMapping: ClassMap<OrdenCompra>
    {
        public OrdenCompraMapping()
        {
            Table("OrdenCompra");
            Id(x => x.nOrdenCompra).GeneratedBy.Identity();
            Map(x => x.detalleOrdenCompra);
            Map(x => x.totalPagar);
            References(x => x.ordenEstado)
                .Column("OrdenEstado")
                .Cascade.None();
        }
    }
}
