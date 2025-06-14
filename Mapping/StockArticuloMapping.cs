using FluentNHibernate.Mapping;
using Proyect_InvOperativa.Models;

namespace Proyect_InvOperativa.Mapping
{
    public class StockArticuloMapping:ClassMap<StockArticulos>
    {
        public StockArticuloMapping()
        {
            Table("StockArticulo");
            Id(x => x.nStock)
                .Column("nStock")
                .GeneratedBy.Identity();
            Map(x => x.stockActual);
            Map(x => x.stockSeguridad);
            Map(x => x.fechaStockInicio);
            Map(x => x.fechaStockFin);

            References(x => x.articulo)
          .Column("idArticulo")
          .Not.Nullable(); 



        }
    }
}
