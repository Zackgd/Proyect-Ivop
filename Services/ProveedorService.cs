using Proyect_InvOperativa.Dtos.Proveedor;
using Proyect_InvOperativa.Models;
using Proyect_InvOperativa.Repository;

namespace Proyect_InvOperativa.Services
{
    public class ProveedorService
    {
        private readonly ProveedoresRepository _proveedoresRepository;
        private readonly ProveedorArticuloRepository _proveedorArticuloRepository;
        private readonly ArticuloRepository _articuloRepository;
        private readonly MaestroArticulosRepository _maestroArticuloRepository;
        private readonly ProveedorEstadoRepository _proveedorEstadoRepository;

        public ProveedorService(ProveedorEstadoRepository proveedorEstadoRepository, ArticuloRepository articuloRepository, ProveedoresRepository proveedoresRepository, ProveedorArticuloRepository proveedorArticulo, MaestroArticulosRepository maestroArticulosRepository)
        {
            _proveedorEstadoRepository = proveedorEstadoRepository;
            _articuloRepository = articuloRepository;
            _proveedoresRepository = proveedoresRepository;
            _proveedorArticuloRepository = proveedorArticulo;
            _maestroArticuloRepository = maestroArticulosRepository;
        }


        #region ABM Proveedores


        public async Task<Proveedor> CreateProveedor(ProveedorDto ProveedorDto)
        {
            var maestro = await _maestroArticuloRepository.GetByIdAsync(1);
            var EstProv = await _proveedorEstadoRepository.GetByIdAsync(1);//revisar
            var proveedor_n = new Proveedor()
            {
                nombreProveedor = ProveedorDto.nombreProveedor,
                idProveedor = ProveedorDto.idProveedor,
                masterArticulo = maestro,
            };
            var estadoProveedor = new EstadoProveedores()
            {
                nEstado = 1,
                fechaIEstadoProveedor = DateTime.UtcNow,
                fechaFEstadoProveedor = null,
                proveedor = proveedor_n,
                proveedorEstado = EstProv
            };

            var newProveedor = await _proveedoresRepository.AddAsync(proveedor_n);

            return newProveedor;
        }

        public async Task UpdateProveedor(long idProveedor, ProveedorDto updateProveedorDto)
        {
            var proveedorModificado = await _proveedoresRepository.GetByIdAsync(idProveedor);
            if (proveedorModificado is null)
            {
                throw new Exception($"Proveedor con id: {idProveedor} no encontrado. ");
            }

            proveedorModificado.nombreProveedor = updateProveedorDto.nombreProveedor;

            await _proveedoresRepository.UpdateAsync(proveedorModificado);

        }

        public async Task DeleteProveedor(long idProveedor)
        {
            var EstProv = await _proveedorEstadoRepository.GetByIdAsync(3);
            var provEliminar = await _proveedoresRepository.GetByIdAsync(idProveedor);
            if (provEliminar is null)
            {
                throw new Exception($"Proveedor con id: {idProveedor} no encontrado. ");
            }
            // await _proveedoresRepository.DeleteIdAsync(idProveedor);
            var estadoProveedor = new EstadoProveedores()
            {
                nEstado = 3,
                fechaIEstadoProveedor = DateTime.UtcNow,
                fechaFEstadoProveedor = null,
                proveedor = provEliminar,
                proveedorEstado = EstProv
            };
            await _proveedoresRepository.UpdateAsync(provEliminar);
        }

        public async Task SuspenderProveedor(long idProveedor)
        {
            var EstProv = await _proveedorEstadoRepository.GetByIdAsync(2);
            var provSuspender = await _proveedoresRepository.GetByIdAsync(idProveedor);
            if (provSuspender is null)
            {
                throw new Exception($"Proveedor con id: {idProveedor} no encontrado. ");
            }
            var estadoProveedor = new EstadoProveedores()
            {
                nEstado = 2,
                fechaIEstadoProveedor = DateTime.UtcNow,
                fechaFEstadoProveedor = null,
                proveedor = provSuspender,
                proveedorEstado = EstProv
            };
            await _proveedoresRepository.UpdateAsync(provSuspender);
        }

        public async Task<IEnumerable<Proveedor>> GetAllProveedores()
        {
            var proveedores = await _proveedoresRepository.GetAllAsync();

            return proveedores;
        }

        public async Task<Proveedor> GetProveedorById(long idProveedor)
        {
            var proveedor = await _proveedoresRepository.GetByIdAsync(idProveedor);

            return proveedor;
        }
        #endregion
    }
}


