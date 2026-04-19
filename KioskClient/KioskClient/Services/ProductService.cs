using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;
using System.Net.Http.Json;

using KioskClient.Models;

namespace KioskClient.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _client;

        public ProductService()
        {
            HttpClientHandler handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            _client = new HttpClient(handler);
            _client.BaseAddress = new Uri("https://localhost:7183/");
        }

        public async Task<List<Product>?> GetProductsAsync()
        {
            return await _client.GetFromJsonAsync<List<Product>>("api/products");
        }

        public async Task<bool> CreateProductAsync(CreateProductRequest request)
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync("api/products", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateProductAsync(int id, UpdateProductRequest request)
        {
            HttpResponseMessage response = await _client.PutAsJsonAsync($"api/products/{id}", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            HttpResponseMessage response = await _client.DeleteAsync($"api/products/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
