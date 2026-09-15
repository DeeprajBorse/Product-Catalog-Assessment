using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Domain.Entities
{
    public class Product
    {
        public int Id   { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public string? UpdatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedOn { get; set;} = DateTime.Now;

        public ICollection<Item> Items { get; set; } = new List<Item>();
    }
}
