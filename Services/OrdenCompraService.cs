using Proyect_InvOperativa.Repository;

namespace Proyect_InvOperativa.Services
{
    public class OrdenCompraService
    {
        #region Lista de Tareas
        //Alta - Modificacion - Baja
        //Gestion de Estado --> Esto se ve pesado 
        #endregion

        private readonly OrdenCompraRepository _ordenCompraRepository;
        private readonly OrdenCompraEstadoRepository _ordenCompraEstadoRepository;

        public OrdenCompraService( OrdenCompraRepository ordenCompraRepository,OrdenCompraEstadoRepository ordenCompraEstadoRepository)
        {
            _ordenCompraRepository = ordenCompraRepository;
            _ordenCompraEstadoRepository = ordenCompraEstadoRepository;
        }

            #region Cancelar orden de compra
            public async Task CancelarOrdenCompra(long nOrdenCompra)
            {
                var ordenC = await _ordenCompraRepository.GetOrdenCompraYDetalles(nOrdenCompra);
                if (ordenC == null) throw new Exception($"Orden de compra con número {nOrdenCompra} no encontrada ");

                // verificar estado actual
                var estActual = ordenC.ordenEstado?.nombreEstadoOrden;
                if (estActual == null || 
                estActual.Equals("Archivada", StringComparison.OrdinalIgnoreCase) || 
                estActual.Equals("Cancelada", StringComparison.OrdinalIgnoreCase) ||
                estActual.Equals("En proceso", StringComparison.OrdinalIgnoreCase) ||
                estActual.Equals("Enviada", StringComparison.OrdinalIgnoreCase)
                )
                {
                throw new Exception("No se puede cancelar la orden de compra ");
                }

                // obtener estado `cancelada`
                var estCancelada = await _ordenCompraRepository.GetEstadoOrdenCompra("Cancelada");
                if (estCancelada == null || estCancelada.fechaFinEstadoDisponible != null) throw new Exception("No se encontró el estado 'Cancelada' ");

                //asignar nuevo estado
                 ordenC.ordenEstado = estCancelada;
                 await _ordenCompraRepository.UpdateAsync(ordenC);
            }
        #endregion
    }
}
