namespace Proyect_InvOperativa.Models
{
    public class Proveedor
    {
        public string nombreProveedor { get; set; } = "";
        public long idProveedor { get; set; }
        public ListaProveedores? listaProveedores { get; set; }
        public MaestroArticulo? masterArticulo { get; set; }


    }
}
