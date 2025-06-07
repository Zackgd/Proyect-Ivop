namespace Proyect_InvOperativa.Dtos.Articulo
{
    public class StockArticuloDto
    {
        public  long nStock { get; set; }
        public int stockSeguridad { get; set; }
        public int stockActual { get; set; }
        public DateTime fechaStockInicio { get; set; }
        public DateTime fechaStockFin { get; set; }
        public long idArticulo { get; set; }
    }
}
