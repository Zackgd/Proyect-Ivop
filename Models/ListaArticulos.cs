namespace Proyect_InvOperativa.Models
{
    public class ListaArticulos
    {
        public virtual long idListaArticulos {  get; set; }
        public virtual DateTime fechaInicioLista { get; set; }
        public virtual DateTime? fechaFinLista { get; set; }
        public virtual Proveedor? proveedor { get; set; }

    }
}
