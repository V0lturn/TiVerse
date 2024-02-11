using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiVerse.Application.Interfaces.IRepositoryInterface
{
    public interface ITiVerseIRepository<T> where T : class
    {
        IQueryable<T> GetAll<T>() where T : class; 
        Task<T> GetById<T>(object id) where T : class;
        void Add<T>(T entity) where T : class;
        void Update<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        void SaveChanges();
    }
}
