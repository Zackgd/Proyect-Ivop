namespace Proyect_InvOperativa.Models
{
    public class DetalleOrdenCompra
    {
        public int nDetalleOrdenCompra { get; set; }
        public int cantidadArticulos { get; set; }
        public long precioSubTotal { get; set; }
        public OrdenCompra? ordenCompra { get; set; }

        public Articulo? articulo { get; set; }



    }
}
