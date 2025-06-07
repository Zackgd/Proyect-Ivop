namespace Proyect_InvOperativa.Dtos.Articulo
{
    public class ListaArticuloDto
    {
        public  long idListaArticulos { get; set; }
        public DateTime fechaInicioLista { get; set; }
        public DateTime fechaFinLista { get; set; }
        public long idproveedor { get; set; }
    }
}
