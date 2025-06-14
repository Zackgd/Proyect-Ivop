using Proyect_InvOperativa.Repository;
using Proyect_InvOperativa.Dtos;
using Proyect_InvOperativa.Dtos.OrdenCompra;
using Proyect_InvOperativa.Models;

namespace Proyect_InvOperativa.Services
{
    public class OrdenCompraEstadoService
    {
        public OrdenCompraEstadoRepository _OCrepository;
        public OrdenCompraEstadoService(OrdenCompraEstadoRepository ordencita)
        {
            _OCrepository = ordencita;
        }


        public async Task CreateOrdenCompraEstado(OrdenCompraEstadosDto dtoOrdenEstado)
        {
            var ordeEstadoNueva = new OrdenCompraEstado()
            {
                nombreEstadoOrden = dtoOrdenEstado.nombreEstadoOrden,
                idEstadoOrdenCompra = dtoOrdenEstado.idEstadoOrdenCompra,
                fechaFinEstadoDisponible = null,
            };
            await _OCrepository.AddAsync(ordeEstadoNueva);
        }

        public async Task DeleteOrdenCompraEstado(OrdenCompraEstadosDto ocEdto)
        {
            var ordenCEstado = await _OCrepository.GetByIdAsync(ocEdto.idEstadoOrdenCompra);

            ordenCEstado.fechaFinEstadoDisponible = DateTime.UtcNow;

            await _OCrepository.UpdateAsync(ordenCEstado);
        }
     
    }
}
