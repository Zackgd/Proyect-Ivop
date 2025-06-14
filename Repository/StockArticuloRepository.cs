using NHibernate;
using Proyect_InvOperativa.Models;

namespace Proyect_InvOperativa.Repository
{
    public class StockArticuloRepository : BaseRepository<StockArticulos>
    {
        //private readonly ISessionFactory _sessionFactory;
        public StockArticuloRepository(ISessionFactory sessionFactory) : base(sessionFactory)
        {
            //_sessionFactory = sessionFactory;
        }
        public StockArticulos? getstockActualbyIdArticulo(long idArticulo)
        {
            using (var session = _sessionFactory.OpenSession())
            {
                return session.Query<StockArticulos>()
            .FirstOrDefault(s => s.articulo!.idArticulo == idArticulo && s.fechaStockFin == null);
            }
        }
    }
}
