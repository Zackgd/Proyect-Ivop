namespace Proyect_InvOperativa.Dtos.OrdenCompra
{
    public class OrdenCompraEstadosDto
    {
        public string nombreEstadoOrden { get; set; } = "";
        public long idEstadoOrdenCompra { get; set; }
        public DateTime? fechaFinEstadoDisponible { get; set; }
        
        
    }
}
