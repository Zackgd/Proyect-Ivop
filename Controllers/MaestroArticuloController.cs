using Microsoft.AspNetCore.Mvc;
using Proyect_InvOperativa.Dtos.Articulo;
using Proyect_InvOperativa.Dtos.MaestroArticulo;
using Proyect_InvOperativa.Services;

namespace Proyect_InvOperativa.Controllers
{
    
    [ApiController]
    [Route("[controller]")]
    public class MaestroArticuloController : ControllerBase
    {
        public readonly MaestroArticulosService _masterArt;

        public MaestroArticuloController(MaestroArticulosService masterArt)
        {
            _masterArt = masterArt;
        }

        #region Artículo
        [HttpPost("articulo")]
        public async Task<IActionResult> CreateArticulo([FromBody] CreateArticuloDto createArticuloDto)
        {
            
            var newArticulo = await _masterArt.CreateArticulo(createArticuloDto.articuloDto, createArticuloDto.stockArticulosDto);

            return Ok(newArticulo);

        }

        [HttpDelete("articulo/{id}")]
        public async Task<IActionResult> DeleteArticulo(long idArticulo)
        {
            await _masterArt.DeleteArticulo(idArticulo);

            return Ok("Artículo eliminado. ");

        }

        [HttpPut("articulo/{id}")]
        public async Task<IActionResult> UpdateArticulo(long idArticulo, [FromBody] ArticuloDto updateArticuloDto)
        {
            await _masterArt.UpdateArticulo(idArticulo, updateArticuloDto);

            return Ok("Artículo modificado. ");

        }

        [HttpGet("articulo")]
        public async Task<IActionResult> GetAllArticulos()
        {
            var articulos = await _masterArt.GetAllArticulos();

            return Ok(articulos);
        }

        [HttpGet("articulo/{id}")]
        public async Task<IActionResult> GetArticuloById(long idArticulo)
        {
            var articulo = await _masterArt.GetArticuloById(idArticulo);

            return Ok(articulo);
        }
        #endregion

        # region StockArticulo

        [HttpPut("articulo/stock/{id}")]
        public async Task<IActionResult> UpdateStockArticulos(long idStockArticulos, [FromBody] StockArticulosDto stock)
        {
            await _masterArt.UpdateStockArticulosAsync(idStockArticulos, stock);
            return NoContent();
        }

        [HttpGet("articulo/stock/{id}")]
        public async Task<IActionResult> GetStockByArticuloId(long idArticulo)
        {
            var stock = await _masterArt.GetStockArticulosByArticuloId(idArticulo);

            return Ok(stock);
        }

        # endregion

        #region MaestroArticulo 

        [HttpPost()]
        public async Task<IActionResult> CreateMaestroArticulo([FromBody] CreateMaestroArticuloDto createMaestroArticuloDto)
        {
            var maestro = await _masterArt.CreateMaestroArticulo(createMaestroArticuloDto);

            return Ok(maestro);

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaestroArticulo(long idMaestroArticulo)
        {
            await _masterArt.DeleteMaestroArticulo(idMaestroArticulo);

            return Ok("Maestro Artículo dado de baja. ");
        }
        #endregion

        

    }
}
