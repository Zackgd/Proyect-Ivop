using NHibernate;
using NHibernate.Linq;
using Proyect_InvOperativa.Dtos.Ventas;
using Proyect_InvOperativa.Models;

namespace Proyect_InvOperativa.Repository
{
    public class VentasRepository : BaseRepository<Ventas>
    {
        public VentasRepository(ISessionFactory sessionFactory) : base(sessionFactory)
        {

        }

        public async Task<List<VentaDetalle>> GetVentasDetallePorArticulo(long idArticulo)
        {
            using var session = _sessionFactory.OpenSession();

            return await session.Query<VentaDetalle>()
             .Where(vdet => vdet.articulo!.idArticulo == idArticulo)
             .Fetch(vdet => vdet.venta)
             .ToListAsync();
        }
        public async Task<VentasDto?> GetDetalleVentaByVenta(long nVenta)
        {
            using var session = _sessionFactory.OpenSession();

            var venta = await session.Query<Ventas>()
                .Where(v => v.nVenta == nVenta)
                .FirstOrDefaultAsync();

            if (venta == null) return null;


            var detalles = await session.Query<VentaDetalle>()
                .Where(vd => vd.venta!.nVenta == nVenta)
                .Fetch(vd => vd.articulo)
                .ToListAsync();


            var ventaDto = new VentasDto
            {
                nVenta = venta.nVenta,
                descripcionVenta = venta.descripcionVenta,
                totalVenta = venta.totalVenta,
                detalles = detalles.Select(d => new DetalleVentasDto
                {
                    idArticulo = d.articulo!.idArticulo,
                    cantidadArticulo = d.cantidad,

                }).ToArray()
            };

            return ventaDto;
        }

    }
}
