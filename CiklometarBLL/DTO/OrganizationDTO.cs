using CiklometarDAL.Models;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace CiklometarBLL.DTO
{
   public class OrganizationDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Point Location { get; set; }

        
    }
}
