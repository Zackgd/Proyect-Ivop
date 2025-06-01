namespace Proyect_InvOperativa.Dtos.Articulo
{
    public class ArticuloDto
    {
        public virtual long idArticulo { get; set; } 
        public virtual string? nombreArticulo { get; set; }
        public virtual string descripcion { get; set; } = "";
        public virtual long idListaArticulo { get; set; }
        public virtual long idMaster { get; set; }

    }
}
