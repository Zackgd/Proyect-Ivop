using NHibernate;
using Proyect_InvOperativa.Models;

namespace Proyect_InvOperativa.Repository
{
    public class VentasRepository : BaseRepository<Ventas>
    {
        public VentasRepository(ISessionFactory sessionFactory) : base(sessionFactory)
        {

        }
    
    }
}
