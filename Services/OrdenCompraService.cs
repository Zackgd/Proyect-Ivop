using Microsoft.AspNetCore.Mvc;
using Proyect_InvOperativa.Dtos.Articulo;
using Proyect_InvOperativa.Dtos.OrdenCompra;
using Proyect_InvOperativa.Models;
using Proyect_InvOperativa.Models.Enums;
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
        private readonly DetalleOrdenCompraRepository _detalleOrdenCompraRepository;
        private readonly ArticuloRepository _articuloRepository;
        private readonly ProveedoresRepository _proveedorRepository;
        private readonly EstadoProveedoresRepository _estProveedorRepository;
        private readonly ProveedorArticuloRepository _proveedorArtRepository;
        private readonly ProveedorArticuloService _proveedorArtService;
        private readonly StockArticuloRepository _stockarticuloRepository;

        public OrdenCompraService( OrdenCompraRepository ordenCompraRepository,EstadoProveedoresRepository proveedorEstadoRepository,OrdenCompraEstadoRepository ordenCompraEstadoRepository,DetalleOrdenCompraRepository detalleOrdenCompraRepository,ArticuloRepository articuloRepository,ProveedoresRepository proveedoresRepository,ProveedorArticuloRepository proveedorArtRepository,StockArticuloRepository stockarticuloRepository,ProveedorArticuloService proveedorArticuloService)
        {
            _ordenCompraRepository = ordenCompraRepository;
            _ordenCompraEstadoRepository = ordenCompraEstadoRepository;
            _detalleOrdenCompraRepository = detalleOrdenCompraRepository;
            _estProveedorRepository = proveedorEstadoRepository;
            _articuloRepository = articuloRepository;
            _proveedorRepository = proveedoresRepository;
            _proveedorArtRepository = proveedorArtRepository;
            _stockarticuloRepository = stockarticuloRepository;
            _proveedorArtService = proveedorArticuloService;
        }

        #region Generar orden de compra
            public async Task<OrdenCompraAvisoDto> GenerarOrdenCompra(List<ArticuloDto> articulos, long idProveedor)
            {
                // obtener proveedor
                var proveedor = await _proveedorRepository.GetByIdAsync(idProveedor);
                if (proveedor == null) throw new Exception($"proveedor con Id {idProveedor} no encontrado ");

                // obtener estado ``Pendiente``
                var estadoPendiente = await _ordenCompraRepository.GetEstadoOrdenCompra("Pendiente");
                if (estadoPendiente == null) throw new Exception("estado 'Pendiente' no encontrado ");

                var artRepetidos = articulos
                .GroupBy(art => art.idArticulo)
                .Where(gr => gr.Count() > 1)
                .Select(gr => gr.Key)
                .ToList();
                if (artRepetidos.Any()){ throw new Exception("se han recibido articulos repetidos: " + string.Join(",", artRepetidos));}

                double totalPagar = 0;
                var resultadoOC = new OrdenCompraAvisoDto();
                var detallesOrden = new List<DetalleOrdenCompra>();
                var avisosPP = new List<string>();
                var avisosOC = new List<string>();

                foreach (var articulosDto in articulos)
                {
                    var articulo = await _articuloRepository.GetByIdAsync(articulosDto.idArticulo);
                    if (articulo == null) continue;
                    var proveedorArt = await _proveedorArtRepository.GetProvArtByIdsAsync(articulo.idArticulo, idProveedor);
                    if (proveedorArt == null) continue;
                    var stock = await _stockarticuloRepository.getstockActualbyIdArticulo(articulo.idArticulo);
                    if (stock == null) throw new Exception($"stock no encontrado para el articulo con Id {articulo.idArticulo} ");
                    long cantidad;
                    double precioUnitario = proveedorArt.precioUnitario;
                    double subTotal;

                    var ordenesVigentesArt = await _ordenCompraRepository.GetOrdenesVigentesArt(articulo.idArticulo, new[] { "Pendiente", "Enviada" });
                    if (ordenesVigentesArt.Any())
                    {
                        avisosOC.Add($" existe al menos una orden de compra Pendiente o Enviada para el articulo '{articulo.nombreArticulo}' (ID {articulo.idArticulo}) ");
                    }

                    if (articulo.modeloInv == ModeloInv.LoteFijo_Q)
                    {
                       cantidad = articulo.qOptimo;
                       subTotal = cantidad * precioUnitario;
                       if ((cantidad+stock.stockActual) < stock.puntoPedido)
                        {
                        avisosPP.Add($"la cantidad ordenada para el articulo '{articulo.nombreArticulo}' (ID {articulo.idArticulo}) actualizará el inventario por debajo del punto de pedido correspondiente ");
                        }
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
                if (!detallesOrden.Any()) throw new Exception($"no se pudo generar ningun detalle para la orden de compra ");;

                // crear orden
                var orden = new OrdenCompra
                {
                    fechaOrden = DateTime.Now,
                    proveedor = proveedor,
                    ordenEstado = estadoPendiente,
                    totalPagar = totalPagar
                };

                await _ordenCompraRepository.AddAsync(orden); // ahora tiene ID generado

                // relacionar y guardar detalles
                foreach (var detalle in detallesOrden)
                {
                    detalle.ordenCompra = orden;
                    await _detalleOrdenCompraRepository.AddAsync(detalle);
                }
                resultadoOC.mensajeOC = "orden de compra generada correctamente ";
                resultadoOC.advertenciasOC_pp = avisosPP;
                resultadoOC.advertenciasOC_oc = avisosOC;
                return resultadoOC;
            }
        #endregion

        #region modificar ordenCompra
            public async Task<OrdenCompraAvisoDto> ModificarOrdenCompra(OrdenCompraModificadaDto ordCModDto)
            {
                var orden = await _ordenCompraRepository.GetOrdenCompraConEstado(ordCModDto.nOrdenCompra);
                if (orden == null) throw new Exception("orden de compra no encontrada ");

                if (orden.ordenEstado == null || !orden.ordenEstado.nombreEstadoOrden.Equals("Pendiente", StringComparison.OrdinalIgnoreCase)) throw new Exception("solo se pueden modificar ordenes en estado 'Pendiente' ");

                // validar proveedor
                var proveedor = await _proveedorRepository.GetByIdAsync(ordCModDto.idProveedor);
                var historialProv = await _estProveedorRepository.GetHistorialByProveedorId(ordCModDto.idProveedor);
                var estadoActual = historialProv.FirstOrDefault(estP => estP.fechaFEstadoProveedor == null);
                if (proveedor == null || estadoActual == null || estadoActual.proveedorEstado == null || estadoActual.proveedorEstado.idEstadoProveedor != 1) throw new Exception("proveedor inhabilitado ");

                var artRepetidos = ordCModDto.articulos
                .GroupBy(art => art.idArticulo)
                .Where(gr => gr.Count() > 1)
                .Select(gr => gr.Key)
                .ToList();
                if (artRepetidos.Any()){  throw new Exception("se han recibido articulos repetidos: " + string.Join(",", artRepetidos));}

                double total = 0;
                var resultadoOC = new OrdenCompraAvisoDto();
                var avisosPP = new List<string>();
                var avisosOC = new List<string>();
                var nDetalles = new List<DetalleOrdenCompra>();

                foreach (var artDto in ordCModDto.articulos)
                {
                    var articulo = await _articuloRepository.GetByIdAsync(artDto.idArticulo);
                    if (articulo == null) continue;

                    var stock = await _stockarticuloRepository.getstockActualbyIdArticulo(articulo.idArticulo);
                    if (stock == null || stock.fechaStockFin != null)   continue;

                    var proveedorArticulo = await _proveedorArtRepository.GetProvArtByIdsAsync(articulo.idArticulo, ordCModDto.idProveedor);
                    if (proveedorArticulo == null)  continue;

                    double precioUnitario = proveedorArticulo.precioUnitario;
                    double subTotal = precioUnitario*artDto.cantidad;
                    total += subTotal;

                    var ordenesVigentesArt = await _ordenCompraRepository.GetOrdenesVigentesArt(articulo.idArticulo, new[] { "Pendiente", "Enviada" });
                    if (ordenesVigentesArt.Any())
                    {
                        avisosOC.Add($" existe al menos una orden de compra Pendiente o Enviada para el articulo '{articulo.nombreArticulo}' (ID {articulo.idArticulo}) ");
                    }

                    if (articulo.modeloInv == ModeloInv.LoteFijo_Q)
                    {
                       if ((artDto.cantidad+stock.stockActual) < stock.puntoPedido)
                        {
                        avisosPP.Add($"la cantidad ordenada para el articulo '{articulo.nombreArticulo}' (ID {articulo.idArticulo}) actualizará el inventario por debajo del punto de pedido correspondiente ");
                        }
                    }

                    nDetalles.Add(new DetalleOrdenCompra
                    {
                        articulo = articulo,
                        cantidadArticulos = artDto.cantidad,
                        precioSubTotal = subTotal,
                        ordenCompra = orden
                    });
                }
                if (!nDetalles.Any()) throw new Exception("no se pudo asociar ningun articulo a la orden ");

                // actualiza proveedor y precio total
                orden.proveedor = proveedor;
                orden.totalPagar = total;
                await _ordenCompraRepository.UpdateAsync(orden);

                // actualiza detalles
                var detallesViejos = await _ordenCompraRepository.GetDetallesByOrdenId(ordCModDto.nOrdenCompra);
                foreach (var detalle in detallesViejos)
                {
                    await _detalleOrdenCompraRepository.DeleteAsync(detalle);
                }

                foreach (var detalle in nDetalles)
                {
                    await _detalleOrdenCompraRepository.AddAsync(detalle);
                }
                resultadoOC.mensajeOC = "orden de compra modificada correctamente ";
                resultadoOC.advertenciasOC_pp = avisosPP;
                resultadoOC.advertenciasOC_oc = avisosOC;
                return resultadoOC;
            }
        #endregion

        #region confirmar OrdenCompra
        public async Task ConfirmarOrdenCompra(long nOrdenCompra)
        {
            var ordenC = await _ordenCompraRepository.GetOrdenCompraConEstado(nOrdenCompra);
            if (ordenC == null) throw new Exception($"orden de compra con nro. {nOrdenCompra} no encontrada ");

            var estActual = ordenC.ordenEstado?.nombreEstadoOrden;
            if (estActual == null || !estActual.Equals("Pendiente", StringComparison.OrdinalIgnoreCase)) throw new Exception("solo se puede confirmar una orden de compra que se encuentra en estado 'Pendiente' ");

            // obtener estado `Enviada`
            var estEnviada = await _ordenCompraRepository.GetEstadoOrdenCompra("Enviada");
            if (estEnviada == null || estEnviada.fechaFinEstadoDisponible != null) throw new Exception("no se encuentra el estado 'Enviada' ");

            // asignar nuevo estado
                ordenC.ordenEstado = estEnviada;
                await _ordenCompraRepository.UpdateAsync(ordenC);
            }
        #endregion

            #region Cancelar orden de compra
            public async Task CancelarOrdenCompra(long nOrdenCompra)
            {
                var ordenC = await _ordenCompraRepository.GetOrdenCompraConEstado(nOrdenCompra);
                if (ordenC == null) throw new Exception($"orden de compra con numero {nOrdenCompra} no encontrada ");

                    // verificar estado actual
                var estActual = ordenC.ordenEstado?.nombreEstadoOrden;
                if (estActual == null ||
                    estActual.Equals("Archivada", StringComparison.OrdinalIgnoreCase) ||
                    estActual.Equals("Cancelada", StringComparison.OrdinalIgnoreCase) ||
                    estActual.Equals("En proceso", StringComparison.OrdinalIgnoreCase) ||
                    estActual.Equals("Enviada", StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception("no se puede cancelar la orden de compra ");
                }

                // obtener estado `cancelada`
                var estCancelada = await _ordenCompraRepository.GetEstadoOrdenCompra("Cancelada");
                if (estCancelada == null || estCancelada.fechaFinEstadoDisponible != null) throw new Exception("no se encontro el estado 'Cancelada' ");

                // asignar nuevo estado
                ordenC.ordenEstado = estCancelada;
                await _ordenCompraRepository.UpdateAsync(ordenC);
            }
        #endregion

        #region cambiar orden de compra a estado "en proceso"
            public async Task OrdenEnProceso(long nOrdenCompra)
            {
                var ordenC = await _ordenCompraRepository.GetOrdenCompraConEstado(nOrdenCompra);
                if (ordenC == null) throw new Exception($"orden de compra con numero {nOrdenCompra} no encontrada ");

                var estActual = ordenC.ordenEstado?.nombreEstadoOrden;
                if (estActual == null || !estActual.Equals("Enviada", StringComparison.OrdinalIgnoreCase)){throw new Exception("cambio permitido unicamente si la orden esta en estado 'Enviada' ");}

                // obtener estado `En proceso`
                var estEnProceso = await _ordenCompraRepository.GetEstadoOrdenCompra("En proceso");
                if (estEnProceso == null || estEnProceso.fechaFinEstadoDisponible != null){throw new Exception("no se encuentra el estado 'En proceso' ");}

                // asignar nuevo estado
                ordenC.ordenEstado = estEnProceso;
                await _ordenCompraRepository.UpdateAsync(ordenC);
            }
        #endregion

        #region RegistrarEntradaArticulos
                public async Task RegistrarEntradaPedido(long nOrdenCompra)
                {
                    // obtener orden de compra
                var ordenC = await _ordenCompraRepository.GetOrdenCompraConEstado(nOrdenCompra);
                if (ordenC == null) throw new Exception($"orden de compra con numero {nOrdenCompra} no encontrada ");

                // validar estado actual
                if (ordenC.ordenEstado == null || !ordenC.ordenEstado.nombreEstadoOrden!.Equals("En proceso", StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception("solo se puede registrar ingreso de ordenes en estado 'En proceso' ");
                }

                // obtener los detalles asociados a la orden
                var detallesOrden = await _ordenCompraRepository.GetDetallesByOrdenId(nOrdenCompra);
                if (detallesOrden == null || !detallesOrden.Any()) throw new Exception("la orden de compra no tiene detalles asociados ");

                // actualizar stock de articulos y controlar estado
                foreach (var detalleC in detallesOrden)
                {
                    var stock = await _stockarticuloRepository.getstockActualbyIdArticulo(detalleC.articulo!.idArticulo);
                    if (stock == null) throw new Exception($"no se encuentra stock para el articulo Id {detalleC.articulo.idArticulo}.");

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
                if (estArchivada == null || estArchivada.fechaFinEstadoDisponible != null) throw new Exception("no se encuentra el estado 'Archivada' ");

                // cambiar estado de la orden
                ordenC.ordenEstado = estArchivada;
                await _ordenCompraRepository.UpdateAsync(ordenC);
            }
            #endregion

            #region listar ordenes por proveedor
            public async Task<List<OrdenCompraDto>> GetOrdenesPorProveedor(long idProveedor)
            {
               var historicoEst = await _estProveedorRepository.GetHistorialByProveedorId(idProveedor);
               var estadoActual = historicoEst.FirstOrDefault(est => est.fechaFEstadoProveedor == null);

                if (estadoActual == null || estadoActual.proveedorEstado == null || estadoActual.proveedorEstado.idEstadoProveedor != 1)
                {
                    throw new Exception("el proveedor no esta dado de alta ");
                }
                var ordenes = await _ordenCompraRepository.GetOrdenesPorProveedor(idProveedor);

                return ordenes.Select(oComp => new OrdenCompraDto
                {
                   nOrdenCompra = oComp.nOrdenCompra,
                   fechaOrden = oComp.fechaOrden,
                   totalPagar = oComp.totalPagar,
                   ordenEstado = oComp.ordenEstado?.nombreEstadoOrden ?? "no definido"
                }).ToList();
            }
            #endregion


            #region  listar ordenes de compra
            public async Task<List<OrdenCompraMostrarDto>> GetOrdenesCompraLista()
            {
                var ordenes = await _ordenCompraRepository.GetOrdenesConEstadoYProveedor();
                return ordenes.Select(oCompra => new OrdenCompraMostrarDto
                {
                 nOrdenCompra = oCompra.nOrdenCompra,
                    proveedor = oCompra.proveedor?.nombreProveedor ?? "Desconocido",
                    estado = oCompra.ordenEstado?.nombreEstadoOrden ?? "Sin estado",
                    fechaOrden = oCompra.fechaOrden,
                    totalPagar = oCompra.totalPagar
                    }).ToList();
            }
        #endregion

        #region detalles orden compra
        public async Task<IEnumerable<DetalleOrdenCompraDto>> GetDetallesOrdenCompra(long idOrdenCompra)
        {
            var detalles = await _ordenCompraRepository.GetDetallesByOrdenId(idOrdenCompra);

            return detalles.Select(detalle => new DetalleOrdenCompraDto
            {
                nDetalleOrdenCompra = detalle.nDetalleOrdenCompra,
                cantidadArticulos = detalle.cantidadArticulos,
                precioSubTotal = detalle.precioSubTotal,
                idArticulo = detalle.articulo?.idArticulo,
                nombreArticulo = detalle.articulo?.nombreArticulo,
            });
            
        }
        #endregion

    }
}
