using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Proyect_InvOperativa.Services;

namespace Proyect_InvOperativa.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArticuloController :ControllerBase
    {
        public readonly ArticuloService _articuloService;

        public ArticuloController(ArticuloService articuloService)
        {
            _articuloService = articuloService;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create()
        {
            int id = 56487;
            string descripcion = "Notebook Vaio";
            await _articuloService.CreateArticulo(id, descripcion);
            return Ok("CREADO EXITOSAMENTE");
        }
    }
}
