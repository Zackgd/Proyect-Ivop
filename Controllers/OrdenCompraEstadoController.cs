using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Proyect_InvOperativa.Dtos.OrdenCompra;
using System;
namespace Proyect_InvOperativa.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdenCompraEstadoController: ControllerBase
    {


        [HttpPost]
        public async Task<IActionResult> CreateOrdenCompraEstados(CreateOrdenCompraEstados createOrdenCompraEstados)
        {

            return Ok("retorno de prueba");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrdenCompraEstados()
        {

            return Ok("retorno de prueba");
        }

    } 
}
