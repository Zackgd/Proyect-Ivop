using Proyect_InvOperativa.Dtos.Articulo;
using Proyect_InvOperativa.Dtos.MaestroArticulo;
using Proyect_InvOperativa.Dtos.Proveedor;
using Proyect_InvOperativa.Models;
using Proyect_InvOperativa.Models.Enums;
using Proyect_InvOperativa.Utils;
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
        private readonly OrdenCompraService _ordenCompraService;
        private readonly OrdenCompraEstadoRepository _ordenCompraEstadoRepository;
        private readonly MaestroArticulosRepository _maestroArticuloRepository;
        private readonly StockArticuloRepository _stockArticuloRepository;
        private readonly ProveedorArticuloRepository _proveedorArticuloRepository;
        private readonly ProveedorArticuloService _proveedorArtService;
      
        public MaestroArticulosService(ArticuloRepository articuloRepository, ProveedoresRepository proveedorRepository,OrdenCompraRepository ordenCompraRepository,OrdenCompraService ordenCompraService,OrdenCompraEstadoRepository ordenCompraEstadoRepository, MaestroArticulosRepository maestroArticulosRepository,StockArticuloRepository stockRepo,ProveedorArticuloRepository PARepository,ProveedorArticuloService proveedorArticuloService)
        {
            _articuloRepository = articuloRepository;
            _proveedorRepository = proveedorRepository;
            _ordenCompraRepository = ordenCompraRepository;
            _ordenCompraService = ordenCompraService;
            _ordenCompraEstadoRepository = ordenCompraEstadoRepository;
            _maestroArticuloRepository = maestroArticulosRepository;
            _stockArticuloRepository=stockRepo;
            _proveedorArticuloRepository = PARepository;
            _proveedorArtService = proveedorArticuloService;
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
               demandaDiaria = ArticuloDto.demandaDiaria,
               costoAlmacen = ArticuloDto.costoAlmacen,
               tiempoRevision = ArticuloDto.tiempoRevision,
               categoriaArt = (CategoriaArt)ArticuloDto.categoriaArt,
               modeloInv = (ModeloInv)ArticuloDto.modeloInv,
               masterArticulo = maestro
            };

            var newArticulo = await _articuloRepository.AddAsync(articulo);

            var articuloStock = new StockArticulos()
            {
                stockSeguridad = 0,
                stockActual = 0,
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
            //var stockAsociadoArticulo = await _stockArticuloRepository.getstockActualbyIdArticulo(ArticuloDto.idArticulo); //falta testear

            if (articuloModificado is null)
            {
                throw new Exception($"Artículo con id: {ArticuloDto.idArticulo} no encontrado. ");
            }
            // MODIFICAR LOS DATOS PROPIOS DE ARTICULO
            articuloModificado.nombreArticulo=ArticuloDto.nombreArticulo;
            articuloModificado.descripcion = ArticuloDto.descripcion;
            articuloModificado.demandaDiaria= ArticuloDto.demandaDiaria;
            articuloModificado.costoAlmacen= ArticuloDto.costoAlmacen;
            articuloModificado.tiempoRevision= ArticuloDto.tiempoRevision;
            articuloModificado.modeloInv= (ModeloInv)ArticuloDto.modeloInv;
            articuloModificado.categoriaArt=(CategoriaArt)ArticuloDto.categoriaArt;
            // MODIFICAR LOS DATOS PROPIOS DE STOCK ASOCIADO A ARTICULO, si es que se pueden 
            
            await _articuloRepository.UpdateAsync(articuloModificado);
            //await _stockArticuloRepository.UpdateAsync(stockAsociadoArticulo!);

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

                // selecciona proveedor predeterminado
                var proveedorArt = proveedoresArticulo.FirstOrDefault(pPred => pPred.predeterminado);
                if (proveedorArt == null) continue; // si no hay proveedor predeterminado no se puede calcular

                // parametros para calculo
                double dProm = articulo.demandaDiaria;
                double demandaAnual = dProm*365;
                double L = proveedorArt.tiempoEntregaDias; 
                double costoPedido = proveedorArt.costoPedido;
                double costoAlmacen = articulo.costoAlmacen;
                var (Z,valSigma) = ModInventarioUtils.ObtenerZySigma(articulo.categoriaArt, L);

                // calculo EOQ
                double qOpt = Math.Sqrt((2*demandaAnual*costoPedido)/costoAlmacen);
                long qOptEnt = (long)Math.Ceiling(qOpt);

                // calc. stock de Seguridad
                double stockSeguridad = Z*valSigma*Math.Sqrt(L);
                long stockSeguridadEnt = (long)Math.Ceiling(stockSeguridad);
                double puntoPedido = stockSeguridad+(dProm*L);
                long puntoPedidoEnt = (long)Math.Ceiling(puntoPedido);
                // obtener StockArticulos 
                var stock = await _stockArticuloRepository.getstockActualbyIdArticulo(articulo.idArticulo);
                if (stock == null) continue;

                stock.stockSeguridad = stockSeguridadEnt;
                stock.puntoPedido = puntoPedidoEnt;
                articulo.qOptimo = qOptEnt;
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

                    var proveedoresArticulo = await _proveedorArticuloRepository.GetByArticuloIdAsync(articulo.idArticulo);
                    if (!proveedoresArticulo.Any()) continue;


                // selecciona proveedor predeterminado
                var proveedorArt = proveedoresArticulo.FirstOrDefault(pPred => pPred.predeterminado);
                if (proveedorArt == null) continue; //

                    long cantidadAPedir = await _proveedorArtService.CalcCantidadAPedirP(articulo, proveedorArt);
                    if (cantidadAPedir == 0) continue;

                    double demandaAnual = articulo.demandaDiaria*365;
                    double costoUnidad = proveedorArt.precioUnitario;
                    double costoPedido = proveedorArt.costoPedido;
                    double costoAlmacen = articulo.costoAlmacen;

                    double cgi = CalcularCGI(demandaAnual, costoUnidad, cantidadAPedir, costoPedido, costoAlmacen);
                    articulo.cgi = cgi;
                    await _articuloRepository.UpdateAsync(articulo);
                }
            }

            public async Task ControlStockPeriodico(CancellationToken cancellationToken)
            {
                var articulos = await _articuloRepository.GetAllAsync();

                foreach (var articulo in articulos)
                {
                    if (cancellationToken.IsCancellationRequested) break;
                    if (articulo.modeloInv != ModeloInv.PeriodoFijo_P) continue;

                    var stockArticulo = await _stockArticuloRepository.getstockActualbyIdArticulo(articulo.idArticulo);
                    if (stockArticulo == null) continue;

                var proveedoresArticulo = await _proveedorArticuloRepository.GetByArticuloIdAsync(articulo.idArticulo);
                if (!proveedoresArticulo.Any()) continue;

                // selecciona proveedor predeterminado
                var proveedorArt = proveedoresArticulo.FirstOrDefault(pPred => pPred.predeterminado);
                if (proveedorArt == null) continue; 
                long idProv = proveedorArt.proveedor.idProveedor;

                    // control por fecha de revisión
                    if (articulo.fechaRevisionP.HasValue)
                    {
                        TimeSpan tiempo = TimeSpan.FromDays(articulo.tiempoRevision);
                        DateTime proximaRevision = articulo.fechaRevisionP.Value.Add(tiempo);
                        if (DateTime.Now < proximaRevision) continue;
                    }
                        var articulo_p = new List<Articulo> { articulo };
                        await _ordenCompraService.GenerarOrdenCompra(articulo_p,idProv);

                    // actualizar fecha de revisión
                    articulo.fechaRevisionP = DateTime.Now;
                    await _articuloRepository.UpdateAsync(articulo);
                }
            }
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

        #region Proveedor Predeterminado
            public async Task<string> EstablecerProveedorPredeterminadoAsync(long idArticulo, long idProveedor)
            {
                // obtener proveedores del articulo
                var proveedoresArticulo = await _proveedorArticuloRepository.GetByArticuloIdAsync(idArticulo);
                if (proveedoresArticulo == null || !proveedoresArticulo.Any()) return $"el articulo con Id {idArticulo} no tiene proveedores asignados ";

                var proveedorActual = proveedoresArticulo.FirstOrDefault(pAct => pAct.proveedor.idProveedor == idProveedor);
                if (proveedorActual == null) return $"el proveedor con ID {idProveedor} no esta asociado al articulo con Id {idArticulo} ";

                // salir si el proveedor ya es predeterminado
                if (proveedorActual.predeterminado) return "este proveedor ya esta definido como predeterminado para el articulo ";

                // buscar el proveedor predeterminado actual
                var provPredActual = proveedoresArticulo.FirstOrDefault(pPred => pPred.predeterminado);
                if (provPredActual != null)
                {
                    provPredActual.predeterminado = false;
                    await _proveedorArticuloRepository.UpdateAsync(provPredActual);
                }

                // Establecer el nuevo proveedor predeterminado
                proveedorActual.predeterminado = true;
                await _proveedorArticuloRepository.UpdateAsync(proveedorActual);

                return $"el proveedor con ID {idProveedor} fue establecido como predeterminado para el articulo con ID {idArticulo} ";
            }
        #endregion

        #region Lista productos a reponer

            public async Task<List<ArticuloStockReposicionDto>> ListarArticulosAReponer()
            {
                var articulos = await _articuloRepository.GetAllAsync();
                var listaArticulosReposicion = new List<ArticuloStockReposicionDto>();

                foreach (var articulo in articulos)
                {
                    if (articulo.modeloInv != ModeloInv.LoteFijo_Q) continue;

                    var stock = await _stockArticuloRepository.getstockActualbyIdArticulo(articulo.idArticulo);
                    if (stock == null) continue;

                    // verifica si el stock actual está por debajo del punto de pedido
                    if (stock.stockActual > stock.puntoPedido) continue;

                    // verificar si no existe una orden vigente
                    var estados = new[] { "Pendiente", "Enviada" };
                    bool ordenVigente = await _ordenCompraRepository.GetOrdenActual(articulo.idArticulo, estados);

                    if (!ordenVigente)
                    {
                        listaArticulosReposicion.Add(new ArticuloStockReposicionDto
                        {
                            IdArticulo = articulo.idArticulo,
                            NombreArticulo = articulo.nombreArticulo ?? "",
                            StockActual = stock.stockActual,
                            PuntoPedido = stock.puntoPedido
                        });
                    }
                }
                return listaArticulosReposicion;
            }
        #endregion

        #region Lista articulos Faltantes
            public async Task<List<ArticuloStockReposicionDto>> ListarArticulosFaltantes()
{
                var articulos = await _articuloRepository.GetAllAsync();
                var listaArticulosFaltantes = new List<ArticuloStockReposicionDto>();

                foreach (var articulo in articulos)
                {
                    var stock = await _stockArticuloRepository.getstockActualbyIdArticulo(articulo.idArticulo);
                    if (stock == null) continue;

                    // Verifica si el stock actual está por debajo del stock de seguridad
                    if (stock.stockActual < stock.stockSeguridad)
                    {
                        listaArticulosFaltantes.Add(new ArticuloStockReposicionDto
                        {
                            IdArticulo = articulo.idArticulo,
                            NombreArticulo = articulo.nombreArticulo ?? "",
                            StockActual = stock.stockActual,
                            StockSeguridad = stock.stockSeguridad
                        });
                    }
                }
                return listaArticulosFaltantes;
            }
        #endregion

        #region Lista proveedores por articulo
            public async Task<List<ProveedoresPorArticuloDto>> ListarProveedoresPorArticulo(long idArticulo)
            {
                var proveedoresArticulo = await _proveedorArticuloRepository.GetByArticuloIdAsync(idArticulo);
                var listaProveedoresDto = new List<ProveedoresPorArticuloDto>();

                foreach (var proveedorArt in proveedoresArticulo)
                {
                    var proveedor = proveedorArt.proveedor;
                    if (proveedor == null) continue;

                    listaProveedoresDto.Add(new ProveedoresPorArticuloDto
                    {
                        idProveedor = proveedor.idProveedor,
                        nombreProveedor = proveedor.nombreProveedor ?? "",
                        emailProveedor = proveedor.mail ?? "",
                        telProveedor = proveedor.telefono ?? "",
                        direccionProveedor = proveedor.direccion ?? "",
                        precioUnitario = proveedorArt.precioUnitario,
                        costoPedido = proveedorArt.costoPedido,
                        tiempoEntregaDias = proveedorArt.tiempoEntregaDias,
                        predeterminado = proveedorArt.predeterminado
                    });
                }
                return listaProveedoresDto;
            }  
        #endregion

        
}   
}
