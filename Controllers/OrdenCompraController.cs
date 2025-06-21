using Microsoft.AspNetCore.Mvc;
using Proyect_InvOperativa.Services;

namespace Proyect_InvOperativa.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdenCompraController: ControllerBase
    {

        private readonly OrdenCompraService _ordenCompraService;
        public OrdenCompraController(OrdenCompraService ordenCompraService)
        {
            _ordenCompraService = ordenCompraService;
        }

        [HttpPost("cancelar/{nOrdenCompra}")]
        public async Task<IActionResult> CancelarOrdenCompra(long nOrdenCompra)
        {
            await _ordenCompraService.CancelarOrdenCompra(nOrdenCompra);
            return Ok("Orden de compra cancelada exitosamente. ");
        }

        [HttpPost("registrar-entrada/{nOrdenCompra}")]
        public async Task<IActionResult> RegistrarEntradaPedido(long nOrdenCompra)
        {
            await _ordenCompraService.RegistrarEntradaPedido(nOrdenCompra);
            return Ok("Entrada de artículos registrada correctamente. ");
        }

    }
}
