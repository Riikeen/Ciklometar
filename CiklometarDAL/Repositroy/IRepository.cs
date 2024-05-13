using CiklometarDAL.Models;
using CiklometarDAL.Repositroy;
using System;
using System.Linq;


namespace CiklometarDAL.Repository
{
    public interface IRepository<T> : IBasicRepository<T> where T : BaseEntity
    {
        T GetById(Guid id);
    }
}
