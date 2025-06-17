namespace Proyect_InvOperativa.Dtos.Articulo
{
    public class StockArticuloDto
    {
        public long nStock { get; set; }
        public long stockSeguridad { get; set; }
        public long stockActual { get; set; }
        public DateTime fechaStockInicio { get; set; }
        public DateTime fechaStockFin { get; set; }
        public long idArticulo { get; set; }
    }
}
