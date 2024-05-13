using CiklometarDAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace CiklometarBLL.Services
{
  public class ContextDb
    {
        private readonly CiklometarContext _context;
        public ContextDb(CiklometarContext context)
        {
            _context = context;
        }
        public void SaveChanges()
        {
           _context.SaveChanges();
        }
        public void SaveChangesAsync()
        {
            _context.SaveChangesAsync();
        }
       
    }
}
