using CiklometarDAL.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using CiklometarDAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CiklometarDAL.Repositroy
{
  public class BasicRepository<T> : IBasicRepository<T> where T : class
    {
        protected readonly CiklometarContext _context;
        protected readonly DbSet<T> entities;
        public BasicRepository(CiklometarContext context)
        {
            _context = context;
            entities = context.Set<T>();
        }

        public void AddRange(IEnumerable<T> entity)
        {
            entities.AddRange(entity);
        }

        public void Delete(T entity)
        {
            entities.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entity)
        {
            entities.RemoveRange(entity);
        }

        public IQueryable<T> GetAll()
        {
            return entities.AsQueryable();
        }
        public IQueryable<T> GetAllAsReadOnly()
        {
            return entities.AsNoTracking();
        }


        public void Insert(T entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            entities.Add(entity);
        }

        //public void Update(T entity)
        //{
        //    if (entity == null) throw new ArgumentNullException("entity");
        //    entities.Update(entity);
        //}
        public void InsertAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            entities.AddAsync(entity);
        }
        public void Update(T entity)
        {
            entities.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

    }
}
