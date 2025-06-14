using NHibernate;
using Proyect_InvOperativa.Models;

namespace Proyect_InvOperativa.Repository
{
    public class OrdenCompraEstadoRepository:BaseRepository<OrdenCompraEstado>
    {
        public OrdenCompraEstadoRepository(ISessionFactory sessionFactory) : base(sessionFactory)
        {

        }
    
    }
}
