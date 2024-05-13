using System;
using System.Collections.Generic;
using System.Text;

namespace CiklometarDAL.Models
{
   public interface IBaseEntity
    {
        public Guid Id { get; set; }
        
    }
}
