using System;
namespace Proyect_InvOperativa.Models
{
    public class ProveedorEstado
    {
        public string nombreEstadoProveedor { get; set; } = "";
        public long idEstadoProveedor { get; set; }
        public DateTime fechaBajaProveedorEstado { get; set; }
        
        public ProveedorEstado()
        {}
    }
}