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
        private readonly ArticuloRepository _articuloRepository;
        private readonly StockArticuloRepository _stockarticuloRepository;

        public OrdenCompraService( OrdenCompraRepository ordenCompraRepository,OrdenCompraEstadoRepository ordenCompraEstadoRepository,ArticuloRepository articuloRepository,StockArticuloRepository stockarticuloRepository)
        {
            _ordenCompraRepository = ordenCompraRepository;
            _ordenCompraEstadoRepository = ordenCompraEstadoRepository;
            _articuloRepository = articuloRepository;
            _stockarticuloRepository = stockarticuloRepository;
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

        #region RegistrarEntradaArticulos
            public async Task RegistrarEntradaPedido(long nOrdenCompra)
            {
                // obtener orden de compra y sus detalles
                var ordenC = await _ordenCompraRepository.GetOrdenCompraYDetalles(nOrdenCompra);
                if (ordenC == null) throw new Exception($"Orden de compra con número {nOrdenCompra} no encontrada.");

                // validar estado actual
                if (ordenC.ordenEstado == null || !ordenC.ordenEstado.nombreEstadoOrden!.Equals("En proceso", StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception("solo se puede registrar ingreso de ordenes en estado 'En proceso' ");
                }

                // actualizar stock de artículos y controlar estado
                foreach (var detalleC in ordenC.detalleOrdenCompra)
                {
                    var stock = await _stockarticuloRepository.getstockActualbyIdArticulo(detalleC.articulo.idArticulo);
                    if (stock == null) throw new Exception($"no se encontró stock para el articulo Id {detalleC.articulo.idArticulo}.");

                    // sumar cantidad recibida
                    stock.stockActual += detalleC.cantidadArticulos;
                    if (stock.stockActual > stock.stockSeguridad)
                    {
                        stock.control = false;
                    }
                    await _stockarticuloRepository.UpdateAsync(stock);
                }

                // obtener estado `Archivada`
                var estArchivada = await _ordenCompraRepository.GetEstadoOrdenCompra("Archivada");
                if (estArchivada == null || estArchivada.fechaFinEstadoDisponible != null) throw new Exception("no se encontró el estado 'Archivada' ");

                // cambiar estado de la orden
                ordenC.ordenEstado = estArchivada;
                await _ordenCompraRepository.UpdateAsync(ordenC);
            }
            #endregion
    }
}
