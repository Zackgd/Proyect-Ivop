using NHibernate;
using NHibernate.Linq;
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
            .Where(vdet => vdet.articulo.idArticulo == idArticulo)
            .Fetch(vdet => vdet.venta)
            .ToListAsync();
        }
    
    }
}
