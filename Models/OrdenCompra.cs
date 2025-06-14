namespace Proyect_InvOperativa.Models
{
    public class OrdenCompra
    {
        public virtual long nOrdenCompra {  get; set; }
        public virtual string? detalleOrdenCompra {  set; get; }
        public virtual long totalPagar { get; set; }
        public virtual OrdenCompraEstado? ordenEstado { get; set; }
        
    }
}
