using System.Net.Http;
using System.Net.Http.Json;
using System.Printing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using KioskClient.Models;

namespace KioskClient
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient _client;

        public MainWindow()
        {
            InitializeComponent();

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            _client = new HttpClient(handler);
            //밑의 주소의 api/products 로 바로 들어갈수 있게 주소를 받아놔둠
            _client.BaseAddress = new Uri("https://localhost:7183/"); 
        }

        private async void LoadProducts_Click(object sender, RoutedEventArgs e)
        {
            await LoadProductsAsync();
        }

        private async void CreateProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(NameTextBox.Text))
                {
                    MessageBox.Show("상품명을 입력해주세요");
                    return;
                }
                
                if(!int.TryParse(PriceTextBox.Text, out int price))
                {
                    MessageBox.Show("가격은 숫자로 입력해주세요");
                    return;
                }

                CreateaProductRequest request = new CreateaProductRequest
                {
                    Name = NameTextBox.Text,
                    Price = price
                };

                HttpResponseMessage response = await _client.PostAsJsonAsync("api/products", request);

                if(!response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"상품 추가 실패 : {response.StatusCode}");
                    return;
                }
                MessageBox.Show("상품이 추가 되었습니다.");

                NameTextBox.Clear();
                PriceTextBox.Clear();

                await LoadProductsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"상품을 불러오는 중 오류가 발생했습니다.\n{ex.Message}");
            }
        }
        
        private async void UpdateProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Product? selectedProduct = ProductsListBox.SelectedItem as Product;

                if(selectedProduct == null)
                {
                    MessageBox.Show("수정 할 상품을 선택해 주세요");
                    return;
                }

                if(string.IsNullOrWhiteSpace(NameTextBox.Text))
                {
                    MessageBox.Show(("상품명을 입력해주세요"));
                    return;
                }

                if(!int.TryParse(PriceTextBox.Text, out int price))
                {
                    MessageBox.Show("가격은 숫자로 입력해주세요");
                    return;
                }

                if(price <= 0 )
                {
                    MessageBox.Show("가격은 1 이상이어야 합니다.");
                    return;
                }

                UpdateProductRequest request = new UpdateProductRequest
                {
                    Name = NameTextBox.Text,
                    Price = price
                };

                var response = await _client.PutAsJsonAsync($"api/products/{selectedProduct.Id}", request);

                if(!response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"상품 수정 실패 : {response.StatusCode}");
                    return;
                }
                MessageBox.Show("상품이 수정되었습니다.");

                await LoadProductsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"상품 수정 중 오류 발생\n{ex.Message}");
            }
        }

        private async void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Product? selectedProduct = ProductsListBox.SelectedItem as Product;
                if(selectedProduct == null)
                {
                    MessageBox.Show("삭제 할 상품을 선택해 주세요");
                    return;
                }

                MessageBoxResult result = MessageBox.Show("정말 삭제하시겠습니까?", "삭제 확인", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if(result != MessageBoxResult.Yes)
                {
                    return;
                }

                HttpResponseMessage response = await _client.DeleteAsync($"api/products/{selectedProduct.Id}");
                if(!response.IsSuccessStatusCode)
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"상품 삭제 실패 : {response.StatusCode}\n{errorMessage}");
                    return;
                }

                MessageBox.Show("상품이 삭제 되었습니다.");

                NameTextBox.Clear();
                PriceTextBox.Clear();

                await LoadProductsAsync();
            }
            catch(Exception ex)
            {
                MessageBox.Show($"상품 삭제 중 오류 발생\n{ex.Message}");
            }
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                List<Product>? products = await _client.GetFromJsonAsync<List<Product>>("api/products");
                ProductsListBox.ItemsSource = products;
            }
            catch(Exception ex)
            {
                MessageBox.Show($"상품을 불러오는 중 오류가 발생했습니다.\n{ex.Message}");
            }
        }

        private void ProductsListBox_SelectionChanged(object sender, SelectionChangedEventArgs s)
        {
            Product? selectedProduct = ProductsListBox.SelectedItem as Product;

            if(selectedProduct != null)
            {
                NameTextBox.Text = selectedProduct.Name;
                PriceTextBox.Text = selectedProduct.Price.ToString();
            }
        }
    }
}