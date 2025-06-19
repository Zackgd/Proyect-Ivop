using Microsoft.AspNetCore.Mvc;
using Proyect_InvOperativa.Dtos.Ventas;
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
        public async Task<IActionResult> ValidarStock([FromBody] StockDto ventasDto)
        {
            var disponible = await _ventasService.ValidarStockDisponible(ventasDto);
            return Ok(disponible);
        }

        [HttpPost()]
        public async Task<IActionResult> CreateVentas([FromBody] VentasDto ventasDto)
        {
            var result = await _ventasService.CreateVentas(ventasDto);
            return Ok(result);
        }
    }
}
