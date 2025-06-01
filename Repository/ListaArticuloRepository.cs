using NHibernate;
using Proyect_InvOperativa.Models;

namespace Proyect_InvOperativa.Repository
{
    public class ListaArticuloRepository:BaseRepository<ListaArticulos>
    {
        public ListaArticuloRepository(ISessionFactory sessionFactory) : base(sessionFactory)
        {

        }
    }
}
