namespace Proyect_InvOperativa.Dtos.OrdenCompra
{
    public class DetalleOrdenCompraDto
    {
        public long nDetalleOrdenCompra { get; set; }
        public long cantidadArticulos { get; set; }
        public decimal precioSubTotal { get; set; }
        public long? idArticulo { get; set; }
        public string? nombreArticulo { get; set; } = "";
    }
}
