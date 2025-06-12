namespace Proyect_InvOperativa.Dtos.Articulo
{
    public class StockArticulosDto
    {
        public long nStock { get; set; }
        public int stockSeguridad { get; set; } = 0;
        public int stockActual { get; set; } = 0;
        public DateTime fechaStockInicio { get; set; } = DateTime.UtcNow;
        public DateTime? fechaStockFin { get; set; }
        public long? idArticulo { get; set; }
    }
}
