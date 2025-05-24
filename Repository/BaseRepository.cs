using NHibernate;
using NHibernate.Linq;

namespace Proyect_InvOperativa.Repository
{
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        private readonly ISessionFactory _sessionFactory;

        public BaseRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }
        public async Task<T> GetByIdAsync(string id)
        {
            using var session = _sessionFactory.OpenSession();
            return await session.GetAsync<T>(id);
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            using var session = _sessionFactory.OpenSession();
            return await session.Query<T>().ToListAsync();
        }
        public async Task AddAsync(T entity)
        {
            using var session = _sessionFactory.OpenSession();
            using var transaction = session.BeginTransaction();
            try
            {
                await session.SaveAsync(entity);
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteAsync(T entity)
        {
            using var session = _sessionFactory.OpenSession();
            using var transaction = session.BeginTransaction();
            try
            {
                await session.DeleteAsync(entity);
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task DeleteIdAsync(string id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                await DeleteAsync(entity);
            }
        }
        public async Task UpdateAsync(T entity)
        {
            using var session = _sessionFactory.OpenSession();
            using var transaction = session.BeginTransaction();
            try
            {
                await session.UpdateAsync(entity);
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


    }
}

