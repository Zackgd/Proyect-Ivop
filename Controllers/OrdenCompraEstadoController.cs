using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Proyect_InvOperativa.Dtos.OrdenCompra;
using System;
namespace Proyect_InvOperativa.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdenCompraEstadoController
    {


        [HttpPost]
        public async Task<IActionResult> CreateOrdenCompraEstados(CreateOrdenCompraEstados createOrdenCompraEstados)
        {

            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrdenCompraEstados()
        {

            return NoContent();
        }

    } 
}
