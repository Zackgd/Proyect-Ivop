
namespace Proyect_InvOperativa.Models
{
    public class Articulo
    {
        public virtual long idArticulo { get; set; }
        public virtual string? nombreArticulo { get; set; }
        public virtual string descripcion { get; set; } = "";
        public virtual long demandaDiaria { get; set; }
        public virtual double costoAlmacen { get; set; }
        public virtual long tiempoRevision { get; set; }
        public virtual Enum? modeloInv { get; set; }
        public virtual Enum? categoriaArt { get; set; }
        public virtual ProveedorArticulo? proveedorArticulo { get; set; }
        public virtual MaestroArticulo? masterArticulo { get; set; }
        
    }
}
