
namespace Proyect_InvOperativa.Models {
    public class VentaDetalle
    {
        public virtual int cantidad { get; set; }
        public virtual int nDetalleVenta { get; set; }
        public virtual long subTotalVenta { get; set; }
        public virtual Ventas? venta {  get; set; }
        public virtual Articulo? articulo { get; set; }
       
      
    }
}