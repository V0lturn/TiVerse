using Microsoft.EntityFrameworkCore;
using TiVerse.Application.Interfaces.IRepositoryInterface;
using TiVerse.Infrastructure.AppDbContext;

namespace TiVerse.Application.Data
{
    public class TiVerseRepository<T> : ITiVerseIRepository<T> where T : class
    {
        private readonly TiVerseDbContext _dbContext;

        public TiVerseRepository(TiVerseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<T> GetAll<T>() where T : class
        {
            return _dbContext.Set<T>().AsQueryable();
        }

        public async Task<T> GetById<T>(object id) where T : class
        {
            if (id is Guid guidId)
            {
                return await _dbContext.Set<T>().FindAsync(guidId);
            }

            if (id is string stringId && Guid.TryParse(stringId, out var parsedGuid))
            {
                return await _dbContext.Set<T>().FindAsync(parsedGuid);
            }

            return null;
        }

        public void Add<T>(T entity) where T : class
        {
            _dbContext.Set<T>().Add(entity);
        }

        public void Update<T>(T entity) where T : class
        {
            _dbContext.Set<T>().Attach(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public void Delete<T>(T entity) where T : class
        {
            _dbContext.Set<T>().Remove(entity);
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}
