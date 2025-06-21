using NHibernate;
using NHibernate.Linq;
using Proyect_InvOperativa.Repository;
using Proyect_InvOperativa.Models;

namespace Proyect_InvOperativa.Repository
{
    public class ArticuloRepository : BaseRepository<Articulo>
    {
        public ArticuloRepository(ISessionFactory sessionFactory): base(sessionFactory) {}

    }
}

