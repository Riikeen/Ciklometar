using System;
using System.Collections.Generic;
using System.Text;

namespace CiklometarDAL.Models
{
    public class RefreshToken : BaseEntity
    {
        public bool IsUsed { get; set; }
        public string Token { get; set; }
        public Guid UserId { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }
    }
}
