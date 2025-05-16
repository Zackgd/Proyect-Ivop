namespace Proyect_InvOperativa.Models
{
    public class Ventas
    {
        public string? descripcionVenta { get; set; }
        public long nVenta { get; set; }
        public long  totalVenta {get;set;}

        public VentaDetalle? ventaDetalle { get; set; }
        public Ventas()
        {

        }
    }
}
