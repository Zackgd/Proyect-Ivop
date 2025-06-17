using NHibernate;
using NHibernate.Linq;
using Proyect_InvOperativa.Models;

namespace Proyect_InvOperativa.Repository
{
    public class ProveedoresRepository:BaseRepository<Proveedor>
    {
        public ProveedoresRepository(ISessionFactory sessionFactory) : base(sessionFactory) { }
    

    }

} 

