namespace Proyect_InvOperativa.Dtos.Proveedor

{
    public class ProveedorDto
    {
        public  string nombreProveedor { get; set; } = "";
        public long idProveedor { get; set; }
        public long? masterArticulo { get; set; }
    }
}
