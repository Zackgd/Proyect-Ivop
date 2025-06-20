using Proyect_InvOperativa.Dtos.Articulo;
using Proyect_InvOperativa.Models;
using Proyect_InvOperativa.Utils;
using Proyect_InvOperativa.Repository;


namespace Proyect_InvOperativa.Services
{
    public class ProveedorArticuloService
    {
        
        private readonly ProveedoresRepository _proveedoresRepository;
        private readonly ArticuloRepository _articuloRepository;
        private readonly ProveedorArticuloRepository _proveedoresArticuloRepository;
        private readonly StockArticuloRepository _stockArtRepository;

        public ProveedorArticuloService(ProveedoresRepository proveedoresRepository,ArticuloRepository artRepo, ProveedorArticuloRepository  pArtRepo,StockArticuloRepository stockArticuloRepository) 
        {
            _proveedoresRepository = proveedoresRepository;
            _articuloRepository = artRepo;  
            _proveedoresArticuloRepository = pArtRepo;
            _stockArtRepository = stockArticuloRepository;
        }
        public async Task<ProveedorArticulo> CreateProveedorArticulo(ProveedorArticuloDto provArtDto)
        {
            var articulo = await _articuloRepository.GetByIdAsync(provArtDto.idArticulo);
            var proveedor = await _proveedoresRepository.GetByIdAsync(provArtDto.idProveedor);

            var proveedorArticulo = new ProveedorArticulo()
            {
                precioUnitario = provArtDto.precioUnitario,
                costoPedido = provArtDto.costoPedido,
                tiempoEntregaDias = provArtDto.tiempoEntregaDias,
                fechaFinProveedorArticulo = null,
                proveedor = proveedor,
                articulo = articulo
            };
            var listaANew = await _proveedoresArticuloRepository.AddAsync(proveedorArticulo);
            return proveedorArticulo;
        }

        public async Task DeleteProveedorArticulo(long id)
        {
            var proveedorArticulo = await _proveedoresArticuloRepository.GetByIdAsync(id); //revisar mapeo para que busque por id o hacer query
            if (proveedorArticulo == null)
            {
                throw new Exception("No existe la lista");
            }
            proveedorArticulo.fechaFinProveedorArticulo = DateTime.UtcNow;
            await _proveedoresArticuloRepository.UpdateAsync(proveedorArticulo);
        }


        #region calcular cant. a pedir 
            public async Task<long> CalcCantidadAPedirP(Articulo articulo, ProveedorArticulo proveedorArt)
            {
                double dProm = articulo.demandaDiaria;
                double T = articulo.tiempoRevision;
                double L = proveedorArt.tiempoEntregaDias;
                double periodoVulnerable = T + L;

                var (Z,valSigma) = ModInventarioUtils.ObtenerZySigma(articulo.categoriaArt, periodoVulnerable);

                var stock = await _stockArtRepository.getstockActualbyIdArticulo(articulo.idArticulo);
                if (stock == null) return 0;

                double stockSeguridad = Z*valSigma;
                long stockSeguridadEnt = (long)Math.Ceiling(stockSeguridad);

                double q = dProm * periodoVulnerable + stockSeguridad - stock.stockActual;
                long qEnt = (long)Math.Ceiling(q);
                if (qEnt < 0) qEnt = 0;

                // actualizar stock de seguridad
                stock.stockSeguridad = stockSeguridadEnt;
                await _stockArtRepository.UpdateAsync(stock);

                return qEnt;
            }
        #endregion
    }
}
