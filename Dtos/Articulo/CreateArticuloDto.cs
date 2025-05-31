namespace Proyect_InvOperativa.Dtos.Articulo
{
    public class CreateArticuloDto
    {
        public virtual string? nombreArticulo { get; set; }
        public virtual string descripcion { get; set; } = "";
    }
}
