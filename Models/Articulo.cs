
namespace Proyect_InvOperativa.Models
{
    public class Articulo
    {
        public virtual long idArticulo { get; set; }
        public virtual string descripcion { get; set; } = "";
        public virtual IList<StockArticulos>? stockArticulos { get; set; } = new List<StockArticulos>();

        public virtual ListaArticulos? listaArticulos { get; set; }
        public virtual MaestroArticulo? masterArticulo { get; set; }
        
    }
}
