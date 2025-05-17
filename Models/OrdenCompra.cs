namespace Proyect_InvOperativa.Models
{
    public class OrdenCompra
    {
        public int nOrdenCompra {  get; set; }
        public string? detalleOrdenCompra {  set; get; }
        public long totalPagar { get; set; }
        public OrdenCompraEstado? ordenEstado { get; set; }
        
    }
}
