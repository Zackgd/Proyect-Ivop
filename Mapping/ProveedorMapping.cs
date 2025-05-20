using FluentNHibernate.Mapping;
using Proyect_InvOperativa.Models;

namespace Proyect_InvOperativa.Mapping
{
    public class ProveedorMapping:ClassMap<Proveedor>
    {
        public ProveedorMapping()
        {
            Table("Proveedor");
            Id(x => x.idProveedor).GeneratedBy.Identity();
            Map(x => x.nombreProveedor);
            References(x => x.masterArticulo)
                .Column("idMaestroArticulo")
                .Cascade.None();
            References(x => x.listaProveedores)
               .Column("idListaProveedores")
               .Cascade.None();
        }
    }
}
