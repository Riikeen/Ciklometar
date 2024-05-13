using CiklometarDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CiklometarDAL.Repositroy
{
   public interface IBasicRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        void Insert(T entity);
        void Update(T entity);
        void Delete(T entity);
        public IQueryable<T> GetAllAsReadOnly();
        void AddRange(IEnumerable<T> entity);
        void DeleteRange(IEnumerable<T> entity);
        void InsertAsync(T entity);

    }
}
