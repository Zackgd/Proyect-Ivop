namespace Proyect_InvOperativa.Dtos.Articulo
{
    public class CreateArticuloDto
    {
        public required virtual ArticuloDto articuloDto { get; set; }

        public required virtual StockArticulosDto stockArticulosDto { get; set; }

    }
}
