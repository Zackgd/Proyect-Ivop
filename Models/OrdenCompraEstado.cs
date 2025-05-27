namespace Proyect_InvOperativa.Models
{
    public class OrdenCompraEstado
    {
       public virtual string? nombreEstadoOrden {  get; set; }
        public virtual long idEstadoOrdenCompra { get; set; }
        public virtual DateTime fechaFinEstadoDisponible { get; set; }

      
    }
}