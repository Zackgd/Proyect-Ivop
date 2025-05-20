using FluentNHibernate.Mapping;
using Proyect_InvOperativa.Models;

namespace Proyect_InvOperativa.Mapping
{
    public class StockArticuloMapping:ClassMap<StockArticulos>
    {
        public StockArticuloMapping()
        {
            Table("StockArticulo");
            Id(x => x.articulo)
                .Column("idArticulo")
                .GeneratedBy.Foreign("articulo");
            Map(x => x.stockActual);
            Map(x => x.stockSeguridad);
            Map(x => x.fechaStockInicio);
            Map(x => x.fechaStockFin);

            HasOne(x => x.articulo)
                .Constrained(); //relacion uno a uno con clave compartida
          

        }
    }
}
