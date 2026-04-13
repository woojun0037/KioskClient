using System.Net.Http;
using System.Net.Http.Json;
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
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void LoadProducts_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using var client = new HttpClient();

                var products = await client.GetFromJsonAsync<List<Product>>("https://localhost:7183/api/products");

                ProductsListBox.ItemsSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"상품을 불러오는 중 오류가 발생했습니다.\n{ex.Message}");
            }
        }
    }
}