namespace Proyect_InvOperativa.Models
{
    public class DetalleOrdenCompra
    {
        public virtual int nDetalleOrdenCompra { get; set; }
        public virtual int cantidadArticulos { get; set; }
        public virtual long precioSubTotal { get; set; }
        public virtual OrdenCompra? ordenCompra { get; set; }

        public virtual Articulo? articulo { get; set; }



    }
}
