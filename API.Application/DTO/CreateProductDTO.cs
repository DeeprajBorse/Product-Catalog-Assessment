using API.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Application.DTO
{
    public class CreateProductDTO
    {
        public string ProductName { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public List<CreateItemDTO> Items { get; set; } = new();
    }
}
