using NHibernate;
using Proyect_InvOperativa.Models;

namespace Proyect_InvOperativa.Repository
{
    public class EstadoProveedoresRepository : BaseRepository<EstadoProveedores>
    {
        public EstadoProveedoresRepository(ISessionFactory sessionFactory) : base(sessionFactory) { }
    }
}
