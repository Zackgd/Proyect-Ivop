namespace Proyect_InvOperativa.Models
{
    public class OrdenCompraEstado
    {
       public string? nombreEstadoOrden {  get; set; }
        public long idEstadoOrdenCompra { get; set; }
        public DateTime fechaFinEstadoDisponible { get; set; }

        public OrdenCompraEstado() { }
    }
}