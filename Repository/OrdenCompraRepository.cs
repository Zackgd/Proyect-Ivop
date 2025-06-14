using NHibernate;
using Proyect_InvOperativa.Models;

namespace Proyect_InvOperativa.Repository
{
    public class OrdenCompraRepository : BaseRepository<OrdenCompra>
    {
        public OrdenCompraRepository(ISessionFactory sessionFactory) : base(sessionFactory)
        {

        }
    
    }
}
