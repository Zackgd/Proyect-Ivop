using Microsoft.AspNetCore.Mvc;
using Proyect_InvOperativa.Dtos.OrdenCompra;
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


    [HttpPost("generar-orden")]
    public async Task<IActionResult> GenerarOrdenCompra([FromBody] OrdenCompraGeneradaDto ordenC_Pedidos)
    {   
        try
        {
            var resultadoOC = await _ordenCompraService.GenerarOrdenCompra(ordenC_Pedidos.articulos, ordenC_Pedidos.idProveedor);
            return Ok(resultadoOC);
        }
        catch (Exception ex){return BadRequest(new {error = ex.Message});}
}

    [HttpPut("modificar-orden")]
    public async Task<IActionResult> ModificarOrdenCompra([FromBody] OrdenCompraModificadaDto ordenModDto)
    {   
        try
        {
            var resultadoOCM = await _ordenCompraService.ModificarOrdenCompra(ordenModDto);
            return Ok(resultadoOCM);
        }
        catch (Exception ex){return BadRequest(new {error = ex.Message});}
    
    }

    [HttpPost("confirmar-orden/{nOrdenCompra}")]
    public async Task<IActionResult> ConfirmarOrdenCompra(long nOrdenCompra)
    {
        try
        {
            await _ordenCompraService.ConfirmarOrdenCompra(nOrdenCompra);
            return Ok(new {mensaje = " orden de compra confirmada y enviada correctamente " });
        }
        catch (Exception ex){return BadRequest(new {error = ex.Message});}
    }

        [HttpPost("cancelar/{nOrdenCompra}")]
        public async Task<IActionResult> CancelarOrdenCompra(long nOrdenCompra)
        {
            await _ordenCompraService.CancelarOrdenCompra(nOrdenCompra);
            return Ok("orden de compra cancelada exitosamente ");
        }

        [HttpPost("orden-enproceso/{nOrdenCompra}")]
        public async Task<IActionResult> OrdenEnProceso(long nOrdenCompra)
        {
             try
                {
                await _ordenCompraService.OrdenEnProceso(nOrdenCompra);
                return Ok(new { mensaje = "cambio a estado 'En proceso' realizado correctamente " });
                }
            catch (Exception ex){return BadRequest(new { error = ex.Message });}
        }

        [HttpPost("registrar-entrada/{nOrdenCompra}")]
        public async Task<IActionResult> RegistrarEntradaPedido(long nOrdenCompra)
        {
            await _ordenCompraService.RegistrarEntradaPedido(nOrdenCompra);
            return Ok("entrada de articulos registrada correctamente ");
        }

        [HttpGet("prov-ord/{idProveedor}")]
        public async Task<ActionResult<List<OrdenCompraDto>>> GetOrdenesPorProveedor(long idProveedor)
        {
                var ordenesP = await _ordenCompraService.GetOrdenesPorProveedor(idProveedor);
                return Ok(ordenesP);
        }
    }
}
