using Proyect_InvOperativa.Models.Enums;

namespace Proyect_InvOperativa.Dtos.Articulo
{
    public class ArticuloDto
    {
        public long idArticulo { get; set; } 
        public string? nombreArticulo { get; set; }
        public string descripcion { get; set; } = "";
        public ModeloInv modeloInv { get; set; }
        public long demandaDiaria { get; set; }
        public double costoAlmacen { get; set; }
        public CategoriaArt categoriaArt { get; set; }
        public long tiemporevision {get; set;}
        public long idMaster { get; set; }
        public long nStock { get; set; }
        public long stockSeguridad { get; set; }
        public long stockActual { get; set; }


    }
}
