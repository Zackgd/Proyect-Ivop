using Proyect_InvOperativa.Models;

namespace Proyect_InvOperativa.Dtos.OrdenCompra
{
    public class OrdenCompraDetalleDto
    {
        public long idArticulo { get; set; }
        public string nombreArticulo { get; set; }
        public long cantidad { get; set; }
        public double precioUnitario { get; set; }
        public double subTotal { get; set; }
        
    }
}
