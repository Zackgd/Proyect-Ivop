namespace Proyect_InvOperativa.Models
{
    public class StockArticulos
    {
        public int stockSeguridad {  get; set; }
        public int stockActual {  get; set; }
        public DateTime fechaStockInicio { get; set; }
        public DateTime fechaStockFin { get; set; }
        public Articulo? articulo { get; set; }

        public StockArticulos() { }
    }
}
