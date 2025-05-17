namespace Proyect_InvOperativa.Models
{
    public class ListaArticulos
    {
        public long idListaArticulos {  get; set; }
        public DateTime fechaInicioLista { get; set; }
        public DateTime fechaFinLista { get; set; }
        public Proveedor? proveedor { get; set; }

    }
}
