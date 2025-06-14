namespace Proyect_InvOperativa.Models
{
    public class ProveedorArticulo
    {
        public virtual double precioUnitario { get; set; }
        public virtual double costoPedido { get; set; }
        public virtual long tiempoEntregaDias { get; set; }
        public virtual DateTime? fechaFinProveedorArticulo { get; set; }
        public virtual Articulo? articulo { get; set; }
        public virtual Proveedor? proveedor { get; set; }

    }
}
