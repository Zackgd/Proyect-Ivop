using Microsoft.AspNetCore.Mvc;
using Proyect_InvOperativa.Dtos.Proveedor;
using Proyect_InvOperativa.Services;
using System;
using System.Web;

namespace Proyect_InvOperativa.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ProveedorEstadoController : ControllerBase
    {
        private readonly ProveedorEstadoService _proveedorEstadoService;

        public ProveedorEstadoController(ProveedorEstadoService proveedorEstadoService)
        {
            _proveedorEstadoService = proveedorEstadoService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProveedorEstado([FromBody] ProveedorEstadoDto dto)
        {
            var result = await _proveedorEstadoService.CreateProveedorEstado(dto);
            return Ok(result); // Podrías usar CreatedAtAction si implementás un GET
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> DeleteProveedorEstado(long id)
        {
            await _proveedorEstadoService.DeleteProveedorEstado(id);
            return NoContent();
        }
    }

}