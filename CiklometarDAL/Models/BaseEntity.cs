using System;
using System.Collections.Generic;
using System.Text;

namespace CiklometarDAL.Models
{
    public class BaseEntity : IBaseEntity
    {
        public Guid Id { get; set; }
        
    }
}
