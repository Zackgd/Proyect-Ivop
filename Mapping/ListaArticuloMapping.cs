using FluentNHibernate.Mapping;
using Proyect_InvOperativa.Models;

namespace Proyect_InvOperativa.Mapping
{
    public class ListaArticuloMapping: ClassMap<ListaArticulos>
    {
        public ListaArticuloMapping()
        {
            Table("ListaArticulos");
            Id(x => x.idListaArticulos).GeneratedBy.Identity();
            Map(x => x.fechaInicioLista);
            Map(x => x.fechaFinLista);
            References(x => x.proveedor)
                .Column("proveedor")
                .Cascade.None();
        }
    }
}
