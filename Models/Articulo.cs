
namespace Proyect_InvOperativa.Models
{
    public class Articulo
    {
        public virtual long idArticulo { get; set; }
        public virtual string? nombreArticulo { get; set; }
        public virtual string descripcion { get; set; } = "";

        public virtual StockArticulos? StockArticulos { get; set; }

        public virtual long idListaArticulos { get; set; }
        public virtual ListaArticulos? listaArticulos { get; set; }

        public virtual long idMaestroArticulo { get; set; }
        public virtual MaestroArticulo? maestroArticulo { get; set; }
        
    }
}
