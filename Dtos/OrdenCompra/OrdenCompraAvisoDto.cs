using Proyect_InvOperativa.Models;

namespace Proyect_InvOperativa.Dtos.OrdenCompra
{
        public class OrdenCompraAvisoDto
        {
                public string mensajeOC { get; set; } = "";
                public List<string> advertenciasOC { get; set; } = new();
        }
}
