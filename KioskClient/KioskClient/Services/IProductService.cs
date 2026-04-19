using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KioskClient.Models;

namespace KioskClient.Services
{
    public interface IProductService
    {
        Task<List<Product>?> GetProductsAsync();
        Task<bool> CreateProductAsync(CreateProductRequest request);
        Task<bool> UpdateProductAsync(int id, UpdateProductRequest request);
        Task<bool> DeleteProductAsync(int id);
    }
}
