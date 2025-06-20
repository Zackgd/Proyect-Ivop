using NHibernate;
using NHibernate.Linq;
using Proyect_InvOperativa.Models;

namespace Proyect_InvOperativa.Repository
{
    public class OrdenCompraRepository : BaseRepository<OrdenCompra>
    {
        public OrdenCompraRepository(ISessionFactory sessionFactory) : base(sessionFactory)
        {

        }

        public async Task<IEnumerable<OrdenCompra>> GetOrdenesVigentesArt(long idArticulo, string[] estadosOrden)
        {
            using var session = _sessionFactory.OpenSession();
            return await session.Query<OrdenCompra>()
           .Where(ordCompra => estadosOrden.Contains(ordCompra.ordenEstado.nombreEstadoOrden)
               && ordCompra.detalleOrdenCompra.Any(detOrden => detOrden.articulo.idArticulo == idArticulo))
           .ToListAsync();
        }

        public async Task<OrdenCompra?> GetOrdenCompraYDetalles(long nOrdenCompra)
            {   
            using var session = _sessionFactory.OpenSession();
            return await session.Query<OrdenCompra>()
                .Fetch(x => x.ordenEstado)
                .FetchMany(x => x.detalleOrdenCompra)
                .ThenFetch(x => x.articulo)
                .FirstOrDefaultAsync(x => x.nOrdenCompra == nOrdenCompra);
            }

        public async Task<OrdenCompraEstado?> GetEstadoOrdenCompra(string nombreEstado)
            {
            using var session = _sessionFactory.OpenSession();
            return await session.Query<OrdenCompraEstado>()
                .FirstOrDefaultAsync(eComp => eComp.nombreEstadoOrden == nombreEstado && eComp.fechaFinEstadoDisponible == null);
            }

        public async Task<bool> GetOrdenActual(long idArticulo, string[] estadosOrden)
            {
            using var session = _sessionFactory.OpenSession();
            var ordP = await session.Query<OrdenCompra>()
                .Where(ordActual => estadosOrden.Contains(ordActual.ordenEstado!.nombreEstadoOrden!)
            && ordActual.detalleOrdenCompra.Any(detOC => detOC.articulo!.idArticulo == idArticulo))
                .AnyAsync();
            return ordP;
            }           

            public async Task<List<DetalleOrdenCompra>> GetDetallesByOrdenId(long nOrdenCompra)
            {
                using var session = _sessionFactory.OpenSession();

                return await session.Query<DetalleOrdenCompra>()
                    .Where(dOrden => dOrden.ordenCompra.nOrdenCompra == nOrdenCompra)
                    .Fetch(dOrden => dOrden.articulo) // opcional: incluye info del artículo
                    .ToListAsync();
            }
    
    }
}
