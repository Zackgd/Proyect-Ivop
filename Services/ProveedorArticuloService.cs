using Proyect_InvOperativa.Dtos.Articulo;
using Proyect_InvOperativa.Models;
using Proyect_InvOperativa.Repository;


namespace Proyect_InvOperativa.Services
{
    public class ProveedorArticuloService
    {
        
        private readonly ProveedoresRepository _proveedoresRepository;
        private readonly ArticuloRepository _articuloRepository;
        private readonly ProveedorArticuloRepository _proveedoresArticuloRepository;
        public ProveedorArticuloService(ProveedoresRepository proveedoresRepository,ArticuloRepository artRepo, ProveedorArticuloRepository  pArtRepo) 
        {
            _proveedoresRepository = proveedoresRepository;
            _articuloRepository = artRepo;  
            _proveedoresArticuloRepository = pArtRepo;
        }
        public async Task<ProveedorArticulo> CreateProveedorArticulo (ProveedorArticuloDto provArtDto)
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

        public async Task DeleteListaArticulo (ProveedorArticuloDto provArtDto)
        {
            var proveedorArticulo = await _proveedoresArticuloRepository.GetByIdAsync(provArtDto.idArticulo); //revisar mapeo para que busque por id o hacer query
            if (proveedorArticulo == null)
            {
                throw new Exception("No existe la lista");
            }
            proveedorArticulo.fechaFinProveedorArticulo = DateTime.UtcNow;
            await _proveedoresArticuloRepository.UpdateAsync(proveedorArticulo);
        }

    }
}
