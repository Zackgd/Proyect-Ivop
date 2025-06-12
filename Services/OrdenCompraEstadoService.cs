using Proyect_InvOperativa.Dtos.OrdenCompra;
using Proyect_InvOperativa.Models;
using Proyect_InvOperativa.Repository;

namespace Proyect_InvOperativa.Services
{
    public class OrdenCompraEstadoService
    {
        private readonly BaseRepository<OrdenCompraEstado> _ordenCompraEstadoRepository;
        public OrdenCompraEstadoService(BaseRepository<OrdenCompraEstado> ordenCompraEstadoRepository)
        {
            _ordenCompraEstadoRepository = ordenCompraEstadoRepository;
        }

        public async Task<IEnumerable<OrdenCompraEstado>> GetAllOrdenCompraEstado()
        {
            return await _ordenCompraEstadoRepository.GetAllAsync();
        }

        public async Task<OrdenCompraEstado> CreateOrdenCompraEstado(OrdenCompraEstadoDto ordenCompraEstadoDto)
        {
            var orden = new OrdenCompraEstado()
            {
                nombreOrdenCompraEstado = ordenCompraEstadoDto.nombreOrdenCompraEstado,
                fechaFinOrdenCompraEstado = null
            };

            var newOrden = await _ordenCompraEstadoRepository.AddAsync(orden);

            return newOrden;

        }

        public async Task DeleteOrdenCompraEstado(long id)
        {
            var estado = await _ordenCompraEstadoRepository.GetByIdAsync(id);
            if (estado is null)
            {
                throw new KeyNotFoundException($"No se encontró estado con ID {id}. ");
            }

            estado.fechaFinOrdenCompraEstado = DateTime.UtcNow;

            await _ordenCompraEstadoRepository.UpdateAsync(estado);
        }


    }
}
