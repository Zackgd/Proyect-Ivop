using NHibernate;
using Proyect_InvOperativa.Models;

namespace Proyect_InvOperativa.Repository
{
    public class OrdenCompraRepository : BaseRepository<ListaArticulos>
    {
        public OrdenCompraRepository(ISessionFactory sessionFactory) : base(sessionFactory)
        {

        }
    {
    }
}
