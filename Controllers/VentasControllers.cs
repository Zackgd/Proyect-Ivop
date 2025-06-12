using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Proyect_InvOperativa.Services;

namespace Proyect_InvOperativa.Controllers
{
    public class VentasControllers: ControllerBase
    {
        public readonly VentasService _ventasService;
        public VentasControllers(VentasService ventitasServices) 
        {
            _ventasService = ventitasServices;
        }
    }
}
