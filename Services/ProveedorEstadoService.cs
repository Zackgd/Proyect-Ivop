﻿using Proyect_InvOperativa.Models;
using Proyect_InvOperativa.Repository;
using Proyect_InvOperativa.Dtos;
using Proyect_InvOperativa.Dtos.Proveedor;
namespace Proyect_InvOperativa.Services

{
    public class ProveedorEstadoService
    {
        public readonly ProveedorEstadoRepository _proveedorEstRepository;

        public ProveedorEstadoService(ProveedorEstadoRepository PERepo)
        {
            _proveedorEstRepository = PERepo;
        }
        public async Task<ProveedorEstado> CreateProveedorEstado(ProveedorEstadoDto proveEstadoDto)
        {
            var proveedorEstadoNuevo = new ProveedorEstado()
            {
                nombreEstadoProveedor = proveEstadoDto.nombreEstadoProveedor,
                idEstadoProveedor = proveEstadoDto.idEstadoProveedor,
                fechaBajaProveedorEstado = null
            };
            var pEstado = await _proveedorEstRepository.AddAsync(proveedorEstadoNuevo);
            return pEstado;
        }

        public async Task UpdateProveedorEstado(long id, ProveedorEstadoDto dto)
        {
            var estadoModificado = await _proveedorEstRepository.GetByIdAsync(id);
            if (estadoModificado == null) {throw new Exception("no se encuentra el estadoProveedor solicitado "); }
            estadoModificado.nombreEstadoProveedor = dto.nombreEstadoProveedor;

            await _proveedorEstRepository.UpdateAsync(estadoModificado);
        }

        public async Task DeleteProveedorEstado(long id)
        {
            var pEstado = await _proveedorEstRepository.GetByIdAsync(id);
            if (pEstado == null)
            {
                throw new Exception("no se encuentra el estadoProveedor solicitado ");
            }
            pEstado.fechaBajaProveedorEstado = DateTime.Now;
            await _proveedorEstRepository.UpdateAsync(pEstado);
        }

        public async Task<IEnumerable<ProveedorEstado>> GetAllEstadosProveedor()
        {
            var estados = await _proveedorEstRepository.GetAllAsync();

            return estados;
        }
    }
}
