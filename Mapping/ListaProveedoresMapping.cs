using FluentNHibernate.Mapping;
using Proyect_InvOperativa.Models;

namespace Proyect_InvOperativa.Mapping
{
    public class ListaProveedoresMapping: ClassMap<ListaProveedores>
    {
        public ListaProveedoresMapping()
        {
            Table("ListaProveedores");
            Id(x => x.idListaProveedores).GeneratedBy.Identity();
            Map(x => x.fechaInicioLProveedores);
            Map(x => x.fechaFinLProveedores);
        }
    }
}
