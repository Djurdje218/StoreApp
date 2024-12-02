using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class Product
    {
        public int id { get; set; }
        public string Name { get; set; }
        public int StoreCode { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
