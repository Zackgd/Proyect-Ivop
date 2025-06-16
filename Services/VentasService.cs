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
            public async Task<bool> ValidarStockDisponible(long idArticulo, long cantidadSolicitada)
            {
                var stockArticulo = await _stockArticuloRepository.getstockActualbyIdArticulo(idArticulo);
                if (stockArticulo == null) return false; 

                return stockArticulo.stockActual >= cantidadSolicitada;
            }

            public async Task ActualizarStockVenta(long idArticulo, long cantidadVendida)
            {
                var articulo = await _articuloRepository.GetByIdAsync(idArticulo);
                if (articulo == null) throw new Exception($"articulo con Id {idArticulo} no encontrado ");

                var stockArticulo = await _stockArticuloRepository.getstockActualbyIdArticulo(idArticulo);
                if (stockArticulo == null) throw new Exception($"no se encuentra stock actual para el artículo con Id {idArticulo} ");

                // actualiza el stock
                stockArticulo.stockActual -= cantidadVendida;

                // si el modelo es Lote Fijo se controla el stock
                if (articulo.modeloInv == ModeloInv.LoteFijo_Q)
                {
                    if (stockArticulo.stockActual <= stockArticulo.stockSeguridad)
                    {
                        stockArticulo.control = true;
                    }
                }

                await _stockArticuloRepository.UpdateAsync(stockArticulo);
            }
        #endregion
    }
}
