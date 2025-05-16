
namespace Proyect_InvOperativa.Models
{
    public class Articulo
    {
        public string idArticulo { get; set; } = "";
        public string descripcion { get; set; } = "";
        public EstadoArticulo? estadoArticulo { get; set; }
        public StockArticulos? stockArticulos { get; set; }
        public Articulo() { }
    }
}
