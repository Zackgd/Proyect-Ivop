namespace Proyect_InvOperativa.Models
{
    public class Ventas
    {
        public virtual string? descripcionVenta { get; set; }
        public virtual long nVenta { get; set; }
        public virtual double totalVenta { get; set; }
        public virtual IList<DetalleVentas> detallesVentas { get; set; } = new List<DetalleVentas>();
        

    }
}
