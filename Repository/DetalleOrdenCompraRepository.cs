using NHibernate;
using NHibernate.Linq;
using Proyect_InvOperativa.Models;

namespace Proyect_InvOperativa.Repository
{
    public class DetalleOrdenCompraRepository : BaseRepository<DetalleOrdenCompra>
    {
        public DetalleOrdenCompraRepository(ISessionFactory sessionFactory) : base(sessionFactory) { }

    public async Task<List<DetalleOrdenCompra>> GetDetallesByOrdenId(long nOrdenCompra)
    {
        using var session = _sessionFactory.OpenSession();

        return await session.Query<DetalleOrdenCompra>()
            .Where(detOC => detOC.ordenCompra.nOrdenCompra == nOrdenCompra)
            .Fetch(detOC => detOC.articulo) 
            .ToListAsync();
    }
    }

    
}

 
