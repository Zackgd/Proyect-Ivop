using ISession = NHibernate.ISession;
using Proyect_InvOperativa.Dtos.Ventas;
using Proyect_InvOperativa.Models;
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
        private readonly BaseRepository<DetalleVentas> _detalleVentasRepository;
        private readonly VentasRepository _ventasRepository;
        private readonly ISession _session;


        public VentasService(StockArticuloRepository stockArticuloRepository, ArticuloRepository articuloRepository, BaseRepository<DetalleVentas> detalleVentasRepository, VentasRepository ventasRepository, ISession session)
        {
            _stockArticuloRepository = stockArticuloRepository;
            _articuloRepository = articuloRepository;
            _detalleVentasRepository = detalleVentasRepository;
            _ventasRepository = ventasRepository;
            _session = session;
        }
        
        #region Actualizar stock (ventas)
        public async Task<bool> ValidarStockDisponible(StockDto ventasDto)
        {
            long idArticulo = ventasDto.idArticulo;
            long cantidadSolicitada = ventasDto.cantidad;
            var stockArticulo = await _stockArticuloRepository.getstockActualbyIdArticulo(idArticulo);

            if (stockArticulo == null) return false; 
            return stockArticulo.stockActual >= cantidadSolicitada;
        }

        private async Task<string?> ActualizarStockVenta(Articulo articulo, DetalleVentas detalle)
        {

            var stockArticulo = await _stockArticuloRepository.getstockActualbyIdArticulo(articulo.idArticulo);
            if (stockArticulo == null) 
                throw new Exception($"no se encuentra stock actual para el artículo con Id {articulo.idArticulo} ");

            // Actualiza el stock restando la cantidad vendida
            stockArticulo.stockActual -= detalle.cantidad;

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
            // await _stockArticuloRepository.UpdateAsync(stockArticulo);
            await _session.UpdateAsync(stockArticulo);
            return aviso_pp;
        }
        #endregion

        #region

        public async Task<Ventas> CreateVentas(VentasDto ventasDto)
        {
            if (ventasDto.detalles.Length < 1)
            {
                throw new Exception("No hay artículos en la venta. ");
            }

            using (var tx = _session.BeginTransaction())
            {
                try
                {
                    var venta = new Ventas
                    {
                        descripcionVenta = ventasDto.descripcionVenta,
                        //totalVenta = 0,
                        detallesVentas = []
                    };

                    //double total = 0;

                    foreach (var detalle in ventasDto.detalles)
                    {
                        var articulo = await _articuloRepository.GetByIdAsync(detalle.idArticulo);
                        if (articulo is null)
                        {
                            throw new Exception($"no se encontró el articulo con Id: {detalle.idArticulo} ");
                        }

                        // con qué calculo el subtotal? esto solo sería el costo de compra nuestro
                        //var subtotal = detalle.cantidadArticulo * articulo.proveedorArticulo!.precioUnitario;

                        var newDetalle = new DetalleVentas
                        {
                            cantidad = detalle.cantidadArticulo,
                            //subTotalVenta = subtotal,
                            venta = venta,
                            articulo = articulo
                        };

                        //total += subtotal;

                        await ActualizarStockVenta(articulo, newDetalle);
                        // await _detalleVentasRepository.AddAsync(newDetalle);
                        await _session.SaveAsync(newDetalle);

                    }

                    //venta.totalVenta = total;

                    // await _ventasRepository.AddAsync(venta);
                    await _session.SaveAsync(venta);

                    await tx.CommitAsync();
                    return venta;
                }
                catch
                {
                    await tx.RollbackAsync();
                    throw;
                }

            }

        }

        #endregion
    }
}
