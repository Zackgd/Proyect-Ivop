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

        #region Articulo
        #endregion
        [HttpPost("articulo/CreateArticulo")]
        public async Task<IActionResult> CreateArticulo(ArticuloDto createArticuloDto)
        {
            var newArticulo = await _masterArt.CreateArticulo(createArticuloDto);

            return Ok(newArticulo);

        }
        [HttpDelete("articulo/DeleteArticulo")]
        public async Task<IActionResult> DeleteArticulo(long idArticulo)
        {
            await _masterArt.DeleteArticulo(idArticulo);

            return Ok("Artículo eliminado. ");

        }
        [HttpPut("articulo/UpdateArticulo")]
        public async Task<IActionResult> UpdateArticulo(ArticuloDto ArticuloDto)
        {
            await _masterArt.UpdateArticulo(ArticuloDto);

            return Ok("Artículo modificado. ");

        }
        [HttpGet("articulo/GetAllArticulos")]
        public async Task<IActionResult> GetAllArticulos()
        {
            var articulos = await _masterArt.GetAllArticulos();

            return Ok(articulos);
        }
        [HttpGet("articulo/GetArticuloById")]
        public async Task<IActionResult> GetArticuloById(long idArticulo)
        {
            var articulo = await _masterArt.GetArticuloById(idArticulo);

            return Ok(articulo);
        }

        #region MaestroArticulo 
        #endregion
        [HttpPost("CreateMaestroArticulo")]
        public async Task<IActionResult> CreateMaestroArticulo(CreateMaestroArticuloDto createMaestroArticuloDto)
        {
            var maestro = await _masterArt.CreateMaestroArticulo(createMaestroArticuloDto);

            return Ok(maestro);

        }
        [HttpDelete("DeleteMaestroArticulo")]
        public async Task<IActionResult> DeleteMaestroArticulo(long idMaestroArticulo)
        {
            await _masterArt.DeleteMaestroArticulo(idMaestroArticulo);

            return Ok("Maestro Artículo dado de baja. ");
        }




    }
}
