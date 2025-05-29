using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("Create")]
        public async Task<IActionResult> Create()
        {
            long id = 56487;
            string descripcion = "Notebook Vaio";
            await _masterArt.CreateArticulo(id, descripcion);
            return Ok("todo bien");
        }
        [HttpPost("Delete")]
        public async Task<IActionResult> Delete()
        {
            long id = 56487;
            long r = await _masterArt.DeleteArticulo(id);
            if(r == 0) { 
            return Ok("Eliminado Correctamente");
            }
            else
            {
                throw new Exception("to mal");
            }
        }
    }
}
