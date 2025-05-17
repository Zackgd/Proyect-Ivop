
namespace Proyect_InvOperativa.Models
{
    public class Articulo
    {
        public string idArticulo { get; set; } = "";
        public string descripcion { get; set; } = "";
        public StockArticulos? stockArticulos { get; set; }
        public ListaArticulos? listaArticulos { get; set; }
        public MaestroArticulo? masterArticulo { get; set; }
        public Articulo() { }
    }
}
