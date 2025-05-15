using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Proyect_InvOperativa.Services;

namespace Proyect_InvOperativa.Controllers
{
    public class VentasControllers
    {
        public readonly VentasService _ventasService;
        public VentasControllers(VentasService ventitasServices) 
        {
            _ventasService = ventitasServices;
        }
        [HttpGet, Route("TestFunction")]
        public async Task<string> funcionPrueba ()
        {
            string mensajePrueba = "HOLA MUNDO";

            var mensajePruebagrande = await _ventasService.getMensajePrueba(mensajePrueba);

            return mensajePruebagrande;
        }
    }
}
