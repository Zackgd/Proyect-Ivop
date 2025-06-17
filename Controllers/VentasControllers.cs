using Microsoft.AspNetCore.Mvc;
using Proyect_InvOperativa.Services;

namespace Proyect_InvOperativa.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VentasController : ControllerBase
    {
        private readonly VentasService _ventasService;

        public VentasController(VentasService ventasService)
        {
            _ventasService = ventasService;
        }

        [HttpGet("validar-stock")]
        public async Task<IActionResult> ValidarStock([FromQuery] long idArticulo, [FromQuery] long cantidad)
        {
            var disponible = await _ventasService.ValidarStockDisponible(idArticulo, cantidad);
            return Ok(disponible);
        }

        [HttpPut("actualizar-stock")]
        public async Task<IActionResult> ActualizarStock([FromQuery] long idArticulo, [FromQuery] long cantidad)
        {
            var mensaje = await _ventasService.ActualizarStockVenta(idArticulo, cantidad);
            return Ok(mensaje);
        }
    }
}
