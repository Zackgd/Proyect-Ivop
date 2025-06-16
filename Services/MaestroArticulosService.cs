using Proyect_InvOperativa.Dtos.Articulo;
using Proyect_InvOperativa.Dtos.MaestroArticulo;
using Proyect_InvOperativa.Models;
using Proyect_InvOperativa.Models.Enums;
using Proyect_InvOperativa.Repository;

namespace Proyect_InvOperativa.Services
{
    #region LISTA DE FALTANTES
    /// <summary>
    //Creacion, modificacion y eliminacion de Articulo (LISTO)
    //    Asignar proveedor Predeterminado
    //    Determinacion del modelo de inventario
    //    Calculo del modelo lote fijo
    //    Calculo del modelo Intervalo fijo
    //    Calculo del CGI
    //    Listado de productos a reponer //// Para hacer el pedido
    //    Listado Productos Faltantes
    //    Listado Proveedores x Articulo
    /// </summary>
    #endregion
    public class MaestroArticulosService
    {
        private readonly ArticuloRepository _articuloRepository;
        private readonly ProveedoresRepository _proveedorRepository;
        private readonly OrdenCompraRepository _ordenCompraRepository;
<<<<<<< HEAD
=======
        private readonly OrdenCompraEstadoRepository _ordenCompraEstadoRepository;
>>>>>>> 2b87e89 (ajustes en modelos de stock)
        private readonly MaestroArticulosRepository _maestroArticuloRepository;
        private readonly StockArticuloRepository _stockArticuloRepository;
        private readonly ProveedorArticuloRepository _proveedorArticuloRepository;
      
<<<<<<< HEAD
        public MaestroArticulosService(ArticuloRepository articuloRepository, ProveedoresRepository proveedorRepository,OrdenCompraRepository ordenCompraRepository, MaestroArticulosRepository maestroArticulosRepository,StockArticuloRepository stockRepo,ProveedorArticuloRepository PARepository)
=======
        public MaestroArticulosService(ArticuloRepository articuloRepository, ProveedoresRepository proveedorRepository,OrdenCompraRepository ordenCompraRepository,OrdenCompraEstadoRepository ordenCompraEstadoRepository, MaestroArticulosRepository maestroArticulosRepository,StockArticuloRepository stockRepo,ProveedorArticuloRepository PARepository)
>>>>>>> 2b87e89 (ajustes en modelos de stock)
        {
            _articuloRepository = articuloRepository;
            _proveedorRepository = proveedorRepository;
            _ordenCompraRepository = ordenCompraRepository;
<<<<<<< HEAD
=======
            _ordenCompraEstadoRepository = ordenCompraEstadoRepository;
>>>>>>> 2b87e89 (ajustes en modelos de stock)
            _maestroArticuloRepository = maestroArticulosRepository;
            _stockArticuloRepository=stockRepo;
            _proveedorArticuloRepository = PARepository;
        }
        #region AB Maestro Articulo
        public async Task<MaestroArticulo> CreateMaestroArticulo(CreateMaestroArticuloDto createMaestroArticuloDto)
        {
            var maestro = new MaestroArticulo()
            {
                idMaestroArticulo = 1,
                nombreMaestro = createMaestroArticuloDto.nombreMaestroArticulo,
            };

            var newMaestro = await _maestroArticuloRepository.AddAsync(maestro);

            return newMaestro;

        }

        public async Task DeleteMaestroArticulo(long idMaestroArticulo)
        {
            var maestroArticulo = await _maestroArticuloRepository.GetByIdAsync(idMaestroArticulo);

            if (maestroArticulo is null)
            {
                throw new Exception($"Artículo con id: {idMaestroArticulo} no encontrado. ");
            }

            await _maestroArticuloRepository.DeleteIdAsync(idMaestroArticulo);
        }
        #endregion

        #region ABM Articulo
        

        public async Task<Articulo> CreateArticulo(ArticuloDto ArticuloDto)
        {
            var maestro = await _maestroArticuloRepository.GetByIdAsync(1); //debe haber otra forma, es para que funcione, despues lo arreglo 
            
            var articulo = new Articulo()
            {
               idArticulo = ArticuloDto.idArticulo,
               nombreArticulo = ArticuloDto.nombreArticulo,
               descripcion = ArticuloDto.descripcion,
               masterArticulo = maestro
            };

            var newArticulo = await _articuloRepository.AddAsync(articulo);

            var articuloStock = new StockArticulos()
            {
                nStock = ArticuloDto.nStock,
                stockSeguridad = ArticuloDto.stockSeguridad,
                stockActual = ArticuloDto.stockActual,
                fechaStockInicio = DateTime.UtcNow,
                fechaStockFin = null,
                articulo = newArticulo // importante: referencia al artículo persistido
            };

            var newArticuloStock = await _stockArticuloRepository.AddAsync(articuloStock);

            return newArticulo;
        }
     

        public async Task UpdateArticulo(ArticuloDto ArticuloDto)
        {
            var articuloModificado = await _articuloRepository.GetByIdAsync(ArticuloDto.idArticulo);
            var stockAsociadoArticulo = await _stockArticuloRepository.getstockActualbyIdArticulo(ArticuloDto.idArticulo); //falta testear

            if (articuloModificado is null)
            {
                throw new Exception($"Artículo con id: {ArticuloDto.idArticulo} no encontrado. ");
            }
            // MODIFICAR LOS DATOS PROPIOS DE ARTICULO
            articuloModificado.descripcion = ArticuloDto.descripcion;
            articuloModificado.demandaDiaria= ArticuloDto.demandaDiaria;
            articuloModificado.costoAlmacen= ArticuloDto.costoAlmacen;
            articuloModificado.tiempoRevision= ArticuloDto.tiemporevision;
            // MODIFICAR LOS DATOS PROPIOS DE STOCK ASOCIADO A ARTICULO, si es que se pueden 
            
            await _articuloRepository.UpdateAsync(articuloModificado);
            await _stockArticuloRepository.UpdateAsync(stockAsociadoArticulo!);

        }

        public async Task DeleteArticulo(long idArticulo)
        {
            
            var artEliminar = await _articuloRepository.GetByIdAsync(idArticulo);

            if (artEliminar is null)
            {
                throw new Exception($"Artículo con id: {idArticulo} no encontrado. ");
            }

            var ordenesVigentesArt = await _ordenCompraRepository.GetOrdenesVigentesArt(idArticulo,new[] { "Pendiente", "Enviada" });

            if (ordenesVigentesArt.Any())
                throw new Exception($"No se puede eliminar el artículo con id: {idArticulo} porque tiene órdenes de compra pendientes o enviadas.");

                    var stockAsociado = await _stockArticuloRepository.getstockActualbyIdArticulo(idArticulo);
                    if (stockAsociado is null)
                    {
                       throw new Exception($"No se encuenctra stock asociado al IDArticulo: {idArticulo}.");
                    }

                        if (stockAsociado.stockActual > 0)
                        throw new Exception($"No se puede eliminar el artículo con id: {idArticulo} porque aún tiene unidades en stock.");

                stockAsociado.fechaStockFin = DateTime.UtcNow;
                await _stockArticuloRepository.UpdateAsync(stockAsociado);
        }

        public async Task<IEnumerable<Articulo>> GetAllArticulos()
        {
            var articulos = await _articuloRepository.GetAllAsync();

            return articulos;
        }

        public async Task<Articulo> GetArticuloById(long idArticulo)
        {
            var articulo = await _articuloRepository.GetByIdAsync(idArticulo);

            return articulo;
        }
        #endregion

        //Metodos para el calculo de Modelo de Inventario
        #region Calculo LoteFijo_Q
        public async Task CalculoLoteFijoQ()
        {
            var articulos = await _articuloRepository.GetAllAsync();

            foreach (var articulo in articulos)
            {
                // verificar si corresponde modelo 
                if (articulo.modeloInv != ModeloInv.LoteFijo_Q) continue;

                // proveedores del articulo
                var proveedoresArticulo = await _proveedorArticuloRepository.GetByArticuloIdAsync(articulo.idArticulo);
                if (!proveedoresArticulo.Any()) continue;

                // proveedor con menor costo unitario
                var proveedorArt = proveedoresArticulo.OrderBy(pMin => pMin.precioUnitario).First();

                // parametros para calculo
                double demanda = articulo.demandaDiaria;
                double demandaAnual = demanda*365;
<<<<<<< HEAD
=======
                double dProm = articulo.demandaDiaria; // demanda diaria promedio
                double L = proveedorArt.tiempoEntregaDias; 
>>>>>>> 2b87e89 (ajustes en modelos de stock)
                double tiempoEntrega = proveedorArt.tiempoEntregaDias;
                double costoPedido = proveedorArt.costoPedido;
                double costoAlmacen = articulo.costoAlmacen;
                var (Z,valSigma) = ObtenerZySigma(articulo.categoriaArt, tiempoEntrega);

                // calculo EOQ
                double qOpt = Math.Sqrt((2*demandaAnual*costoPedido)/costoAlmacen);
                long qOptEnt = (long)Math.Ceiling(qOpt);

                // calc. stock de Seguridad
                double stockSeguridad = Z*valSigma*Math.Sqrt(tiempoEntrega);
<<<<<<< HEAD
                long stockSeguridadEnt = (long)Math.Ceiling(stockSeguridad);
=======
                double puntoPedido = stockSeguridad+(dProm*L);
                long puntoPedidoEnt = (long)Math.Ceiling(puntoPedido);
>>>>>>> 2b87e89 (ajustes en modelos de stock)
                // obtener StockArticulos 
                var stock = await _stockArticuloRepository.getstockActualbyIdArticulo(articulo.idArticulo);
                if (stock == null) continue;

<<<<<<< HEAD
                stock.stockSeguridad = stockSeguridadEnt;
=======
                stock.stockSeguridad = puntoPedidoEnt;
>>>>>>> 2b87e89 (ajustes en modelos de stock)
                await _stockArticuloRepository.UpdateAsync(stock);
                double cgi = CalcularCGI(demandaAnual, proveedorArt.precioUnitario, qOptEnt, costoPedido, costoAlmacen);
                articulo.cgi = cgi;
                await _articuloRepository.UpdateAsync(articulo);
            }

        }
        #endregion

        #region Calculo PeriodoFijo_P
            public async Task CalculoPeriodoFijoP()
            {
                var articulos = await _articuloRepository.GetAllAsync();

                foreach (var articulo in articulos)
                {
                    if (articulo.modeloInv != ModeloInv.PeriodoFijo_P) continue;

<<<<<<< HEAD
                    double dProm = articulo.demandaDiaria; // demanda diaria promedio

                    // proveedores del artículo
                    var proveedoresArticulo = await _proveedorArticuloRepository.GetByArticuloIdAsync(articulo.idArticulo);
                    if (!proveedoresArticulo.Any()) continue;

                    // proveedor con menor precio unitario
                    var proveedorArt = proveedoresArticulo.OrderBy(p => p.precioUnitario).First();

                    double T = articulo.tiempoRevision;               
                    double L = proveedorArt.tiempoEntregaDias;        
                    double periodoVulnerable= T+L;
                    var (Z, sigma) = ObtenerZySigma(articulo.categoriaArt, periodoVulnerable);

                    // stock actual
                    var stock = await _stockArticuloRepository.getstockActualbyIdArticulo(articulo.idArticulo);
                    if (stock == null) continue;

                    // inv. de seguridad
                    double stockSeguridad = Z * sigma;
                    long stockSeguridadEnt = (long)Math.Ceiling(stockSeguridad);

                    // cantidad a pedir
                    double q = dProm*periodoVulnerable + stockSeguridad - stock.stockActual;
                    long qEnt = (long)Math.Ceiling(q);
                    if (qEnt < 0) qEnt = 0;

                    double demandaAnual = dProm*365;
=======
                    var proveedoresArticulo = await _proveedorArticuloRepository.GetByArticuloIdAsync(articulo.idArticulo);
                    if (!proveedoresArticulo.Any()) continue;

                    var proveedorArt = proveedoresArticulo.OrderBy(pArt => pArt.precioUnitario).First();

                    long cantidadAPedir = await CalcCantidadAPedirP(articulo, proveedorArt);
                    if (cantidadAPedir == 0) continue;

                    double demandaAnual = articulo.demandaDiaria*365;
>>>>>>> 2b87e89 (ajustes en modelos de stock)
                    double costoUnidad = proveedorArt.precioUnitario;
                    double costoPedido = proveedorArt.costoPedido;
                    double costoAlmacen = articulo.costoAlmacen;

<<<<<<< HEAD
                    // actualizar stock de seguridad
                    stock.stockSeguridad = stockSeguridadEnt;
                    await _stockArticuloRepository.UpdateAsync(stock);
                    double cgi = CalcularCGI(demandaAnual, costoUnidad, qEnt, costoPedido, costoAlmacen);
=======
                    double cgi = CalcularCGI(demandaAnual, costoUnidad, cantidadAPedir, costoPedido, costoAlmacen);
>>>>>>> 2b87e89 (ajustes en modelos de stock)
                    articulo.cgi = cgi;
                    await _articuloRepository.UpdateAsync(articulo);
                }
            }
<<<<<<< HEAD
=======

            private async Task<long> CalcCantidadAPedirP(Articulo articulo, ProveedorArticulo proveedorArt)
            {
                double dProm = articulo.demandaDiaria;
                double T = articulo.tiempoRevision;
                double L = proveedorArt.tiempoEntregaDias;
                double periodoVulnerable = T + L;

                var (Z, sigma) = ObtenerZySigma(articulo.categoriaArt, periodoVulnerable);

                var stock = await _stockArticuloRepository.getstockActualbyIdArticulo(articulo.idArticulo);
                if (stock == null) return 0;

                double stockSeguridad = Z * sigma;
                long stockSeguridadEnt = (long)Math.Ceiling(stockSeguridad);

                double q = dProm * periodoVulnerable + stockSeguridad - stock.stockActual;
                long qEnt = (long)Math.Ceiling(q);
                if (qEnt < 0) qEnt = 0;

                // actualizar stock de seguridad
                stock.stockSeguridad = stockSeguridadEnt;
                await _stockArticuloRepository.UpdateAsync(stock);

                return qEnt;
            }

            public async Task ControlStockPeriodico(CancellationToken cancellationToken)
            {
               var articulos = await _articuloRepository.GetAllAsync();

                foreach (var articulo in articulos)
                {
                    if (cancellationToken.IsCancellationRequested)  break;

                    if (articulo.modeloInv != ModeloInv.PeriodoFijo_P) continue;

                    var proveedoresArticulo = await _proveedorArticuloRepository.GetByArticuloIdAsync(articulo.idArticulo);
                    if (!proveedoresArticulo.Any()) continue;

                    // proveedor con menor precio unitario
                    var proveedorArt = proveedoresArticulo.OrderBy(pUnitario => pUnitario.precioUnitario).First();

                    // calcular cantidad a pedir
                    long cantidadAPedir = await CalcCantidadAPedirP(articulo, proveedorArt);
                    if (cantidadAPedir == 0) continue;

                    // verificar existencia de orden vigente
                    bool existeOrdenVigente = await _ordenCompraRepository.GetOrdenActual(articulo.idArticulo,new[] { "Pendiente", "Enviada" });

                    if (existeOrdenVigente)  continue;

                    var estadoPendiente = await _ordenCompraEstadoRepository.GetEstado("Pendiente");
                    if (estadoPendiente == null) throw new Exception("Estado 'Pendiente' no encontrado.");

                    var ordenGenerada = new OrdenCompra
                    {
                        detalleOrden = $"Orden generada automaticamente para el articulo {articulo.nombreArticulo}",
                        proveedor = proveedorArt.proveedor,
                        ordenEstado = estadoPendiente,
                        totalPagar = cantidadAPedir * proveedorArt.precioUnitario,
                        detalleOrdenCompra = new List<DetalleOrdenCompra>
                        {
                            new DetalleOrdenCompra
                            {
                                articulo = articulo,
                                cantidadArticulos = cantidadAPedir,
                                precioSubTotal = proveedorArt.precioUnitario
                            }
                        }
                    };

                    await _ordenCompraRepository.AddAsync(ordenGenerada);
                }
            }
>>>>>>> 2b87e89 (ajustes en modelos de stock)
        #endregion

        #region Calculo CostoGlobalInv
            private double CalcularCGI(double demandaAnual, double costoUnidad, double cantidadPedido, double costoPedido, double costoAlmacen)
            {
                if (cantidadPedido <= 0) return 0; 

                double cgi = (demandaAnual * costoUnidad) +
                 ((demandaAnual / cantidadPedido) * costoPedido) +
                 ((cantidadPedido / 2.0) * costoAlmacen);
                return cgi;
            }
        #endregion

        #region val. Z y sigma
            private (double Z, double sigma) ObtenerZySigma(CategoriaArt? categoria, double tiempo_p)
            {
                double Z = 1.64485363; // nivel de servicio esperado (z) para 0,95

                if (categoria == null) throw new ArgumentException("Categoria no encontrada");
                double val_Sigma = categoria switch
                {
                    CategoriaArt.Categoria_A => (6.0+2.0)/2.0,
                    CategoriaArt.Categoria_B => (4.0+1.0)/2.0,
                    CategoriaArt.Categoria_C => (0.2+1.0)/2.0,
                    CategoriaArt.Categoria_D => (0.1+1.0)/2.0,
                    _ => throw new ArgumentException("Categoría no válida")
                };

                double sigma = val_Sigma * Math.Sqrt(tiempo_p);
                return (Z, sigma);
            }
        #endregion

        #region RegistrarEntradaArticulos
            public async Task RegistrarEntradaPedido(long nOrdenCompra)
            {
                // obtener orden de compra y detalles
                var ordenC = await _ordenCompraRepository.GetOrdenCompraYDetalles(nOrdenCompra);
                if (ordenC == null) throw new Exception($"Orden de compra con número {nOrdenCompra} no encontrada.");

                // validar estado actual
                if (ordenC.ordenEstado == null || !ordenC.ordenEstado.nombreEstadoOrden!.Equals("En proceso", StringComparison.OrdinalIgnoreCase))
                {
                throw new Exception("Solo se puede registrar ingreso de órdenes en estado 'En proceso' ");
                }

                // actualiza stock de articulos en la ordenCompra
                foreach (var detalleC in ordenC.detalleOrdenCompra)
                {
                    var stock = await _stockArticuloRepository.getstockActualbyIdArticulo(detalleC.articulo.idArticulo);
                    if (stock == null)
                    throw new Exception($"No se encontró stock para el artículo ID {detalleC.articulo.idArticulo}.");
                    stock.stockActual += detalleC.cantidadArticulos;
                    await _stockArticuloRepository.UpdateAsync(stock);
                }

                // obtener estado `archivada`
                var estArchivada = await _ordenCompraRepository.GetEstadoOrdenCompra("Archivada");
                if (estArchivada == null || estArchivada.fechaFinEstadoDisponible != null) throw new Exception("No se encontró el estado 'Archivada' ");

                //cambiar estado de la orden
                ordenC.ordenEstado = estArchivada;
                await _ordenCompraRepository.UpdateAsync(ordenC);
            }
        #endregion

        #region Cancelar orden de compra
            public async Task CancelarOrdenCompra(long nOrdenCompra)
            {
                var ordenC = await _ordenCompraRepository.GetOrdenCompraYDetalles(nOrdenCompra);
                if (ordenC == null) throw new Exception($"Orden de compra con número {nOrdenCompra} no encontrada ");

                // verificar estado actual
                var estActual = ordenC.ordenEstado?.nombreEstadoOrden;
<<<<<<< HEAD
                if (estActual == null || estActual.Equals("Archivada", StringComparison.OrdinalIgnoreCase) || estActual.Equals("Cancelada", StringComparison.OrdinalIgnoreCase))
                {
                throw new Exception("No se puede cancelar una orden archivada o previamente cancelada ");
=======
                if (estActual == null || 
                estActual.Equals("Archivada", StringComparison.OrdinalIgnoreCase) || 
                estActual.Equals("Cancelada", StringComparison.OrdinalIgnoreCase) ||
                estActual.Equals("En proceso", StringComparison.OrdinalIgnoreCase) ||
                estActual.Equals("Enviada", StringComparison.OrdinalIgnoreCase)
                )
                {
                throw new Exception("No se puede cancelar la orden de compra ");
>>>>>>> 2b87e89 (ajustes en modelos de stock)
                }

                // obtener estado `cancelada`
                var estCancelada = await _ordenCompraRepository.GetEstadoOrdenCompra("Cancelada");
                if (estCancelada == null || estCancelada.fechaFinEstadoDisponible != null) throw new Exception("No se encontró el estado 'Cancelada' ");

                //asignar nuevo estado
                 ordenC.ordenEstado = estCancelada;
                 await _ordenCompraRepository.UpdateAsync(ordenC);
            }
        #endregion
<<<<<<< HEAD
=======

        #region Proveedor Predeterminado
            public async Task<string> EstablecerProveedorPredeterminadoAsync(long idProveedor)
            {
            var proveedorActual = await _proveedorRepository.GetByIdAsync(idProveedor);
            if (proveedorActual == null)
                return $"Proveedor con ID {idProveedor} no encontrado.";

            if (proveedorActual.predeterminado)
            return "Este proveedor ya está definido como predeterminado.";

            // Buscar si ya existe un proveedor predeterminado
            var proveedorPredeterminadoExistente = await _proveedorRepository.GetProveedorPredeterminado();
            if (proveedorPredeterminadoExistente != null)
            {
                proveedorPredeterminadoExistente.predeterminado = false;
                await _proveedorRepository.UpdateAsync(proveedorPredeterminadoExistente);
            }

            // Establecer nuevo proveedor predeterminado
            proveedorActual.predeterminado = true;
            await _proveedorRepository.UpdateAsync(proveedorActual);

            return $"Proveedor con ID {idProveedor} ahora es el predeterminado del sistema.";
        }
        #endregion
>>>>>>> 2b87e89 (ajustes en modelos de stock)
}   
}
