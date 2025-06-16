namespace Proyect_InvOperativa.Models
{
    public class Proveedor
    {
        public virtual string nombreProveedor { get; set; } = "";
        public virtual long idProveedor { get; set; }
        public virtual string? direccion {  get; set; }
        public virtual string? mail { get; set; }
        public virtual string? telefono {  get; set; }
        public virtual MaestroArticulo? masterArticulo { get; set; }
        public virtual IList<ProveedorArticulo> proveedorArticulos { get; set; } = new List<ProveedorArticulo>();
<<<<<<< HEAD

=======
        public virtual bool predeterminado { get; set; }
>>>>>>> 2b87e89 (ajustes en modelos de stock)

    }
}
