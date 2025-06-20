using MySqlX.XDevAPI;
using NHibernate;
using NHibernate.Linq;
using Proyect_InvOperativa.Models;

namespace Proyect_InvOperativa.Repository
{
    public class ProveedorArticuloRepository:BaseRepository<ProveedorArticulo>
    {
        public ProveedorArticuloRepository(ISessionFactory sessionFactory) : base(sessionFactory){}

        public async Task<IEnumerable<ProveedorArticulo>> GetByArticuloIdAsync(long idArticulo)
        {
            using var session = _sessionFactory.OpenSession();

            return await session.Query<ProveedorArticulo>()
            .Where(pArt => pArt.articulo!.idArticulo == idArticulo)
            .ToListAsync();
        }

        public async Task<ProveedorArticulo?> GetProvArtByIdsAsync(long idArticulo, long idProveedor)
        {
            using var session = _sessionFactory.OpenSession();
            return await session.Query<ProveedorArticulo>()
            .FirstOrDefaultAsync(prArt =>
            prArt.articulo!.idArticulo == idArticulo &&
            prArt.proveedor!.idProveedor == idProveedor);
        }       
    }
}
