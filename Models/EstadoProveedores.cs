using System;
namespace Proyect_InvOperativa.Models
{
	public class EstadoProveedores
	{
		public DateTime fechaIEstadoProveedor { get; set; }
		public DateTime fechaFEstadoProveedor { get; set; }
		public ProveedorEstado? proveedorEstado { get; set; }
        public Proveedor? proveedor { get; set; }
        public EstadoProveedores()
		{
		}
	}
}
