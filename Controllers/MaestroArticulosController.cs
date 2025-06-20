using Microsoft.AspNetCore.Mvc;
using Proyect_InvOperativa.Dtos.Articulo;
using Proyect_InvOperativa.Dtos.MaestroArticulo;
using Proyect_InvOperativa.Dtos.Proveedor;
using Proyect_InvOperativa.Services;
using System.Reflection.Metadata.Ecma335;

namespace Proyect_InvOperativa.Controllers
{
    
    [ApiController]
    [Route("[controller]")]
    public class MaestroArticulosController : ControllerBase
    {
        public readonly MaestroArticulosService _maestroArticulosService;

        public MaestroArticulosController(MaestroArticulosService masterArt)
        {
            _maestroArticulosService = masterArt;
        }

        #region Artículo
        
        [HttpPost("articulo")]
        public async Task<IActionResult> CreateArticulo([FromBody] ArticuloDto articuloDto)
        {
            var result = await _maestroArticulosService.CreateArticulo(articuloDto);

            return Ok(result);

        }

        [HttpDelete("articulo/{idArticulo}")]
        public async Task<IActionResult> DeleteArticulo(long idArticulo)
        {
            await _maestroArticulosService.DeleteArticulo(idArticulo);

            return Ok("Artículo eliminado. ");

        }

        [HttpPut("articulo")]
        public async Task<IActionResult> UpdateArticulo([FromBody] ArticuloDto articuloDto)
        {
            await _maestroArticulosService.UpdateArticulo(articuloDto);

            return Ok("Artículo modificado. ");

        }

        [HttpGet("articulo")]
        public async Task<IActionResult> GetAllArticulos()
        {
            var articulos = await _maestroArticulosService.GetAllArticulos();

            return Ok(articulos);
        }

        [HttpGet("articulo/{idArticulo}")]
        public async Task<IActionResult> GetArticuloById(long idArticulo)
        {
            var articulo = await _maestroArticulosService.GetArticuloById(idArticulo);

            return Ok(articulo);
        }

        #endregion

        #region MaestroArticulo 
        [HttpPost("maestro-articulo")]
        public async Task<IActionResult> CreateMaestroArticulo([FromBody] CreateMaestroArticuloDto createMaestroArticuloDto)
        {
            var maestro = await _maestroArticulosService.CreateMaestroArticulo(createMaestroArticuloDto);

            return Ok(maestro);

        }

        [HttpDelete("maestro-articulo/{idMaestroArticulo}")]
        public async Task<IActionResult> DeleteMaestroArticulo(long idMaestroArticulo)
        {
            await _maestroArticulosService.DeleteMaestroArticulo(idMaestroArticulo);

            return Ok("Maestro Artículo dado de baja. ");
        }
        #endregion

        #region Modelo Inventario

        [HttpPost("lote-fijo-q")]
        public async Task<IActionResult> CalcularLoteFijoQ()
        {
            await _maestroArticulosService.CalculoLoteFijoQ();
            return NoContent();
        }

        [HttpPost("periodo-fijo-p")]
        public async Task<IActionResult> CalcularPeriodoFijoP()
        {
            await _maestroArticulosService.CalculoPeriodoFijoP();
            return NoContent();
        }

        [HttpPost("control-stock-periodico")]
        public async Task<IActionResult> ControlStockPeriodico(CancellationToken cancellationToken)
        {
            await _maestroArticulosService.ControlStockPeriodico(cancellationToken);
            return NoContent();
        }

        #endregion

        #region Proveedor Predeterminado
        [HttpPost("articulo/{idArticulo}/proveedores/{idProveedor}/predeterminado")]
        public async Task<IActionResult> EstablecerProveedorPredeterminado(long idArticulo, long idProveedor)
        {
            var mensaje = await _maestroArticulosService.EstablecerProveedorPredeterminadoAsync(idArticulo, idProveedor);
            return Ok(mensaje);
        }
        #endregion

        #region Artículos Listas
        [HttpGet("articulos/a-reponer")]
        public async Task<ActionResult<List<ArticuloStockReposicionDto>>> ListarArticulosAReponer()
        {
            var resultado = await _maestroArticulosService.ListarArticulosAReponer();
            return Ok(resultado);
        }

        [HttpGet("articulos/faltantes")]
        public async Task<ActionResult<List<ArticuloStockReposicionDto>>> ListarArticulosFaltantes()
        {
            var resultado = await _maestroArticulosService.ListarArticulosFaltantes();
            return Ok(resultado);
        }

        [HttpGet("articulo/{idArticulo}/proveedores")]
        public async Task<ActionResult<List<ProveedoresPorArticuloDto>>> ListarProveedoresPorArticulo(long idArticulo)
        {
            var resultado = await _maestroArticulosService.ListarProveedoresPorArticulo(idArticulo);
            return Ok(resultado);
        }
        #endregion


    }
}
