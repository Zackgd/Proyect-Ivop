
namespace Proyect_InvOperativa.Models {
    public class VentaDetalle
    {
        public int cantidad { get; set; }
        public int nDetalleVenta { get; set; }
        public long subTotalVenta { get; set; }
        public Ventas? venta {  get; set; }
        public Articulo? articulo { get; set; }
       
        public VentaDetalle()
        {

        }
    }
}