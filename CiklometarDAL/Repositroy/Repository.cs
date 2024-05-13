using CiklometarDAL.Models;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CiklometarDAL.Repositroy;

namespace CiklometarDAL.Repository
{
   public class Repository<T> : BasicRepository<T>, IRepository<T> where T : BaseEntity
    {
        
        
        public Repository(CiklometarContext context): base(context)
        {
            
        }  
        
        public T GetById(Guid id)
        {
            return entities.SingleOrDefault(s => s.Id == id);
        }
    }
}

