using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Proyect_InvOperativa.Dtos.OrdenCompra;
using Proyect_InvOperativa.Services;
using System;
namespace Proyect_InvOperativa.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdenCompraEstadoController: ControllerBase
    {
        private readonly OrdenCompraEstadoService _ordenCompraEstadoService;
        public OrdenCompraEstadoController(OrdenCompraEstadoService ordenCompraEstadoService)
        {
            _ordenCompraEstadoService = ordenCompraEstadoService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrdenCompraEstados(OrdenCompraEstadoDto ordenCompraEstadoDto)
        {
            var estado = await _ordenCompraEstadoService.CreateOrdenCompraEstado(ordenCompraEstadoDto);
            return Ok(estado);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrdenCompraEstados()
        {
            var estados = await _ordenCompraEstadoService.GetAllOrdenCompraEstado();
            return Ok(estados);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteOrdenCompraEstado(long id)
        {
            await _ordenCompraEstadoService.DeleteOrdenCompraEstado(id);
            return NoContent();
        }

    } 
}
