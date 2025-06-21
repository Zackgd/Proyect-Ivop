using Microsoft.AspNetCore.Mvc;
using Proyect_InvOperativa.Dtos.Articulo;
using Proyect_InvOperativa.Services;

namespace Proyect_InvOperativa.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProveedorArticuloController : ControllerBase
    {
        private readonly ProveedorArticuloService _proveedorArticuloService;

        public ProveedorArticuloController(ProveedorArticuloService proveedorArticuloService)
        {
            _proveedorArticuloService = proveedorArticuloService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProveedorArticulo([FromBody] ProveedorArticuloDto provArtDto)
        {
            var result = await _proveedorArticuloService.CreateProveedorArticulo(provArtDto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeleteProveedorArticulo(long id)
        {
            await _proveedorArticuloService.DeleteProveedorArticulo(id);
            return NoContent();
        }
    }
}