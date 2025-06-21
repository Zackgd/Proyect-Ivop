using NHibernate;
using Proyect_InvOperativa.Models;

namespace Proyect_InvOperativa.Repository
{
    public class ProveedorEstadoRepository : BaseRepository<ProveedorEstado>
    {
        public ProveedorEstadoRepository(ISessionFactory sessionFactory) : base(sessionFactory) { }
    }
}

 
