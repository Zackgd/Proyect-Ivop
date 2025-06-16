using Proyect_InvOperativa.Models.Enums;
using Proyect_InvOperativa.Repository;

namespace Proyect_InvOperativa.Services
{
    public class VentasService
    {
        #region LISTA DE FALTANTES
        //Alta Venta
        #endregion

        private readonly StockArticuloRepository _stockArticuloRepository;
        private readonly ArticuloRepository _articuloRepository;

        public VentasService( StockArticuloRepository stockArticuloRepository,ArticuloRepository articuloRepository)
        {
            _stockArticuloRepository = stockArticuloRepository;
            _articuloRepository = articuloRepository;
        }

        #region Actualizar stock (ventas)
            public async Task<string?> ActualizarStockVenta(long idArticulo, long cantidadVendida)
            {
                var articulo = await _articuloRepository.GetByIdAsync(idArticulo);
                if (articulo == null) throw new Exception($"articulo con ID {idArticulo} no encontrado ");

                 var stockArticulo = await _stockArticuloRepository.getstockActualbyIdArticulo(idArticulo);
                 if (stockArticulo == null) throw new Exception($"no se encuentra stock actual para el artículo con Id {idArticulo} ");

                // Actualiza el stock restando la cantidad vendida
                stockArticulo.stockActual -= cantidadVendida;

                // Actualiza control si el stock cae por debajo del stock de seguridad
                if (stockArticulo.stockActual <= stockArticulo.stockSeguridad)
                {
                    stockArticulo.control = true;
                }

                // Mensaje de advertencia si es modelo Q y se alcanza el punto de pedido
                string? aviso_pp = null;
                if (articulo.modeloInv == ModeloInv.LoteFijo_Q)
                {
                    if (stockArticulo.stockActual <= stockArticulo.puntoPedido)
                    {
                        aviso_pp = $"el articulo '{articulo.nombreArticulo}' alcanzo o esta por debajo del punto de pedido ";
                    }
                }
                await _stockArticuloRepository.UpdateAsync(stockArticulo);
                return aviso_pp;
            }
        #endregion
    }
}
