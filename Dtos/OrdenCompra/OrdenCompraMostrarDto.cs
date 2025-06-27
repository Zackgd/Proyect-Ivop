using Proyect_InvOperativa.Dtos.Articulo;

namespace Proyect_InvOperativa.Dtos.OrdenCompra
{
    public class OrdenCompraMostrarDto
    {
        public long nOrdenCompra { get; set; }
        public string proveedor { get; set; } = "";
        public string estado { get; set; } = "";
        public DateTime? fechaOrden { get; set; }
        public double totalPagar { get; set; }
        
    }
}
