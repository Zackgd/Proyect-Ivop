using NHibernate;
using Proyect_InvOperativa.Models;

namespace Proyect_InvOperativa.Repository
{
    public class VentasRepository : BaseRepository<ListaArticulos>
    {
        public VentasRepository(ISessionFactory sessionFactory) : base(sessionFactory)
        {

        }
    {
    }
}
