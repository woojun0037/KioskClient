using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KioskClient.Models
{
    public class CreateProductRequest
    {
        public string Name { get; set; } = "";
        public int Price { get; set; }
    }
}
