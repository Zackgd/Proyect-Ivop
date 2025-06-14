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
        private readonly MaestroArticulosRepository _maestroArticuloRepository;
        private readonly StockArticuloRepository _stockArticuloRepository;
        private readonly ProveedorArticuloRepository _proveedorArticuloRepository;
      
        public MaestroArticulosService(ArticuloRepository articuloRepository, ProveedoresRepository proveedorRepository, MaestroArticulosRepository maestroArticulosRepository,StockArticuloRepository stockRepo,ProveedorArticuloRepository PARepository)
        {
            _articuloRepository = articuloRepository;
            _proveedorRepository = proveedorRepository;
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
                demandaDiaria = ArticuloDto.demandaDiaria,
                costoAlmacen = ArticuloDto.costoAlmacen,
                tiempoRevision = ArticuloDto.tiemporevision,
                modeloInv = ArticuloDto.modeloInv,
                categoriaArt = ArticuloDto.categoriaArt,
                masterArticulo = maestro
            };
            var articuloStock = new StockArticulos()
            {
                nStock = ArticuloDto.nStock,
                stockSeguridad = ArticuloDto.stockSeguridad,
                stockActual = ArticuloDto.stockActual,
                fechaStockInicio = DateTime.UtcNow,
                fechaStockFin = null,
                articulo = articulo
            };

            var newArticulo = await _articuloRepository.AddAsync(articulo);

            return newArticulo;
        }
     

        public async Task UpdateArticulo(ArticuloDto ArticuloDto)
        {
            var articuloModificado = await _articuloRepository.GetByIdAsync(ArticuloDto.idArticulo);
            var stockAsociadoArticulo = _stockArticuloRepository.getstockActualbyIdArticulo(ArticuloDto.idArticulo); //falta testear

            if (articuloModificado is null )
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

        #region ABM Proveedor
        public async Task CreateProveedor(string nombreP,long idP)
        {
            var proveedor = new Proveedor
            {
                nombreProveedor = nombreP,
                idProveedor = idP,
                masterArticulo = null
            };
            await _proveedorRepository.AddAsync(proveedor);
        }

        public async Task DeleteProveedor(long idProveedor)
        {
           await _proveedorRepository.DeleteIdAsync(idProveedor);
        }
        #endregion


        //Metodos para el calculo de Modelo de Inventario
        public async Task CalculoLoteFijoAsync()
        {
            
            var articulos = await _articuloRepository.GetAllAsync();

            foreach (var articulo in articulos)
            {
                 //verificar si corresponde modelo lote fijo q
                if (articulo.modeloInv!= ModeloInv.LoteFijo_Q)  //esto esta comentado pq me sale error no se puede comparar con el enum
                    continue;

                // Asignar sigma según categoría
                double valSigma = articulo.categoriaArt switch
                {
                    CategoriaArt.Categoria_A => (6.0 + 2.0) / 2.0,
                    CategoriaArt.Categoria_B => (4.0 + 1.0) / 2.0,
                    CategoriaArt.Categoria_C => (0.2 + 1.0) / 2.0,
                    CategoriaArt.Categoria_D => (0.1 + 1.0) / 2.0,
                    _ => throw new ArgumentException("Categoría no válida")
                };

                // obtener proveedor del articulo
                var proveedoresArticulo = await _proveedorArticuloRepository.GetByArticuloIdAsync(articulo.idArticulo);
                if (!proveedoresArticulo.Any())
                    continue;

                // selecciona el proveedor con menor costo unitario
                var proveedorArt = proveedoresArticulo.OrderBy(pMin => pMin.precioUnitario).First();

                // parametros para calculo
                double Z = 1.64485363; // nivel de servicio esperado 0,95
                double demanda = articulo.demandaDiaria;
                double demandaAnual = demanda * 365;
                double tiempoEntrega = proveedorArt.tiempoEntregaDias;
                double costoPedido = proveedorArt.costoPedido;
                double costoAlmacen = articulo.costoAlmacen;


                // calculo EOQ
                double qOpt = Math.Sqrt((2 * demandaAnual * costoPedido) / costoAlmacen);
                long qOptEnt = (long)Math.Ceiling(qOpt);

                // calc. stock de Seguridad
                double stockSeguridad = Z * valSigma * Math.Sqrt(tiempoEntrega);
                long stockSeguridadEnt = (long)Math.Ceiling(stockSeguridad);
                // obtener StockArticulos 
                //var stock = articulo.StockArticulos;
                //if (stock == null)
                //    continue;

                //stock.stockSeguridad = stockSeguridadEnt;
                //await _stockArticulosRepository.UpdateAsync(stock);
            }
        }


        #region Calculo PeriodoFijo_P
       
        #endregion
        public void CalculoIntervaloFijo(){}
        public void CalculoCGI(){}
    }
}
