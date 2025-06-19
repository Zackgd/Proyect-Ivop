using Proyect_InvOperativa.Repository;
using Proyect_InvOperativa.Models;
using Proyect_InvOperativa.Models.Enums;

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
        private readonly DetalleOrdenCompraRepository _detalleOrdenCompraRepository;
        private readonly ArticuloRepository _articuloRepository;
        private readonly ProveedoresRepository _proveedorRepository;
        private readonly ProveedorArticuloRepository _proveedorArtRepository;
        private readonly ProveedorArticuloService _proveedorArtService;
        private readonly StockArticuloRepository _stockarticuloRepository;

        public OrdenCompraService( OrdenCompraRepository ordenCompraRepository,OrdenCompraEstadoRepository ordenCompraEstadoRepository,DetalleOrdenCompraRepository detalleOrdenCompraRepository,ArticuloRepository articuloRepository,ProveedoresRepository proveedoresRepository,ProveedorArticuloRepository proveedorArtRepository,StockArticuloRepository stockarticuloRepository,ProveedorArticuloService proveedorArticuloService)
        {
            _ordenCompraRepository = ordenCompraRepository;
            _ordenCompraEstadoRepository = ordenCompraEstadoRepository;
            _detalleOrdenCompraRepository = detalleOrdenCompraRepository;
            _articuloRepository = articuloRepository;
            _proveedorRepository = proveedoresRepository;
            _proveedorArtRepository = proveedorArtRepository;
            _stockarticuloRepository = stockarticuloRepository;
            _proveedorArtService = proveedorArticuloService;
        }

        #region Generar orden de compra
            public async Task GenerarOrdenCompra(List<Articulo> articulos, long idProveedor)
            {
                // obtener proveedor
                var proveedor = await _proveedorRepository.GetByIdAsync(idProveedor);
                if (proveedor == null) throw new Exception($"Proveedor con Id {idProveedor} no encontrado.");

                // obtener estado ``Pendiente``
                var estadoPendiente = await _ordenCompraRepository.GetEstadoOrdenCompra("Pendiente");
                if (estadoPendiente == null) throw new Exception("Estado 'Pendiente' no encontrado.");

                double totalPagar = 0;
                var detallesOrden = new List<DetalleOrdenCompra>();

                foreach (var articulo in articulos)
                {
                    var proveedorArt = await _proveedorArtRepository.GetProvArtByIdsAsync(articulo.idArticulo, idProveedor);
                    if (proveedorArt == null) continue;
                    long cantidad;
                    double precioUnitario = proveedorArt.precioUnitario;
                    double subTotal;

                    if (articulo.modeloInv == ModeloInv.LoteFijo_Q)
                    {
                    cantidad = articulo.qOptimo;
                    subTotal = cantidad * precioUnitario;
                    }
                    else if (articulo.modeloInv == ModeloInv.PeriodoFijo_P)
                    {
                        cantidad = await _proveedorArtService.CalcCantidadAPedirP(articulo, proveedorArt);
                        if (cantidad == 0) continue;
                        subTotal = cantidad * precioUnitario;
                    }
                    else
                    {
                        continue;
                    }

                    totalPagar += subTotal;
                    var detalle = new DetalleOrdenCompra
                    {
                        articulo = articulo,
                        cantidadArticulos = cantidad,
                        precioSubTotal = subTotal
                    };
                    detallesOrden.Add(detalle);
                }
                if (!detallesOrden.Any()) return;

                // Primero se crea la orden
                var orden = new OrdenCompra
                {
                    fechaOrden = DateTime.Now,
                    proveedor = proveedor,
                    ordenEstado = estadoPendiente,
                    totalPagar = totalPagar
                };

                await _ordenCompraRepository.AddAsync(orden); // ahora tiene ID generado

                // Ahora se relacionan y guardan los detalles
                foreach (var detalle in detallesOrden)
                {
                    detalle.ordenCompra = orden;
                    await _detalleOrdenCompraRepository.AddAsync(detalle);
                }
            }
        #endregion

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
                    // obtener orden de compra
                var ordenC = await _ordenCompraRepository.GetByIdAsync(nOrdenCompra);
                if (ordenC == null) throw new Exception($"Orden de compra con número {nOrdenCompra} no encontrada.");

                // validar estado actual
                if (ordenC.ordenEstado == null || !ordenC.ordenEstado.nombreEstadoOrden!.Equals("En proceso", StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception("Solo se puede registrar ingreso de órdenes en estado 'En proceso'.");
                }

                // obtener los detalles asociados a la orden
                var detallesOrden = await _ordenCompraRepository.GetDetallesByOrdenId(nOrdenCompra);
                if (detallesOrden == null || !detallesOrden.Any()) throw new Exception("La orden de compra no tiene detalles asociados.");

                // actualizar stock de artículos y controlar estado
                foreach (var detalleC in detallesOrden)
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
