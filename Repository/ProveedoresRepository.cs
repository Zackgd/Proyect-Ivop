using NHibernate;
using NHibernate.Linq;
using Proyect_InvOperativa.Models;

namespace Proyect_InvOperativa.Repository
{
    public class ProveedoresRepository:BaseRepository<Proveedor>
    {
        public ProveedoresRepository(ISessionFactory sessionFactory) : base(sessionFactory) { }
    
        public async Task<Proveedor?> GetProveedorPredeterminado()
        {
            using var session = _sessionFactory.OpenSession();
            return await session.Query<Proveedor>()
                .FirstOrDefaultAsync(pPred => pPred.predeterminado == true);
        }
    }

} 

