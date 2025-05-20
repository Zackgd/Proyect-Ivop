using FluentNHibernate.Mapping;
using Proyect_InvOperativa.Models;

namespace Proyect_InvOperativa.Mapping
{
    public class OrdenCompraEstadoMapping: ClassMap<OrdenCompraEstado>
    {
        public OrdenCompraEstadoMapping()
        {
            Table("OrdenCompraEstado");
            Id(x => x.idEstadoOrdenCompra).GeneratedBy.Identity();
            Map(x => x.nombreEstadoOrden);
            Map(x => x.fechaFinEstadoDisponible);
        }
    }
}
