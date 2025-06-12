using Proyect_InvOperativa.Models;
using Proyect_InvOperativa.Repository;
using Proyect_InvOperativa.Dtos;
using Proyect_InvOperativa.Dtos.Proveedor;
namespace Proyect_InvOperativa.Services

{
    public class ProveedorEstadoService
    {
        public readonly ProveedorEstadoRepository _proveedorERepository;

        public ProveedorEstadoService(ProveedorEstadoRepository PERepo)
        {
            _proveedorERepository = PERepo;
        }
        public async Task<ProveedorEstado> CreateProveedorEstado(ProveedorEstadoDto proveEstadoDto)
        {
            var proveedorEstadoNuevo = new ProveedorEstado()
            {
                nombreEstadoProveedor = proveEstadoDto.nombreEstadoProveedor,
                idEstadoProveedor = proveEstadoDto.idEstadoProveedor,
                fechaBajaProveedorEstado = null
            };
            var pEstado = await _proveedorERepository.AddAsync(proveedorEstadoNuevo);
            return pEstado;
        }

        public async Task DeleteProveedorEstado(ProveedorEstadoDto proveEdto)
        {
            var pEstado = await _proveedorERepository.GetByIdAsync(proveEdto.idEstadoProveedor);
            if (pEstado == null)
            {
                throw new Exception("No se encontro el estado para el proveedor");
            }
           pEstado.fechaBajaProveedorEstado = DateTime.Now;
            await _proveedorERepository.UpdateAsync(pEstado);
        }
    }
}
