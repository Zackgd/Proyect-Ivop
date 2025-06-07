using NHibernate;
using Proyect_InvOperativa.Models;

namespace Proyect_InvOperativa.Repository
{
    public class ListaProveedoresRepository : BaseRepository<ListaProveedores>
    {
        public ListaProveedoresRepository(ISessionFactory sessionFactory) : base(sessionFactory) { }
    }
}
