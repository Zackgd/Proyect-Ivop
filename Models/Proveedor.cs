namespace Proyect_InvOperativa.Models
{
    public class Proveedor
    {
        public virtual string nombreProveedor { get; set; } = "";
        public virtual long idProveedor { get; set; }
        public virtual ListaProveedores? listaProveedores { get; set; }
        public virtual MaestroArticulo? masterArticulo { get; set; }


    }
}
