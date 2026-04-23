using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using KioskClient.Models;
using KioskClient.Services;
using KioskClient.Commands;

namespace KioskClient.Models.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IProductService _productService;

        private string _productName  = "";
        private string _productPrice = "";
        private Product _selectedProduct;

        public ObservableCollection<Product> Products { get; } = new ObservableCollection<Product>();

        public string ProductName
        {
            get => _productName;
            set {  _productName = value; OnPropertyChanged();}
        }

        public string ProductPrice 
        { 
            get => _productPrice;
            set { _productPrice = value; OnPropertyChanged();}
        }

        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set 
            {
                _selectedProduct = value;
                OnPropertyChanged();

                if (_selectedProduct != null)
                {
                    ProductName  = _selectedProduct.Name;
                    ProductPrice = _selectedProduct.Price.ToString();
                }
            }
        }

        public ICommand LoadProductCommand   { get; }
        public ICommand CreateProductCommand { get; }
        public ICommand UpdateProductCommand { get; }
        public ICommand DeleteProductCommand { get; }
        public ICommand NewProductCommand { get; }

        public MainViewModel()
        {
            _productService = new ProductService();

            LoadProductCommand   = new RelayCommand(async _ => await LoadProductAsync());
            CreateProductCommand = new RelayCommand(async _ => await CreateProductAsync());
            UpdateProductCommand = new RelayCommand(async _ => await UpdateProductAsync());
            DeleteProductCommand = new RelayCommand(async _ => await DeleteProductAsync());
            NewProductCommand    = new RelayCommand(_ => ClearInput());
        }

        public async Task LoadProductAsync()
        {
            List<Product>? products = await _productService.GetProductsAsync();

            Products.Clear();

            if(products == null)
            {
                return;
            }

            foreach(Product product in products)
            {
                Products.Add(product);
            }
        }

        public async Task CreateProductAsync()
        {
            try
            {
                if(SelectedProduct != null)
                {
                    MessageBox.Show("선택된 상품이 있습니다. 새 상품을 추가하려면 선택을 해제해주세요");
                    return;
                }

                if(string.IsNullOrWhiteSpace(ProductName))
                {
                    MessageBox.Show("상품명을 입력해주세요.");
                    return;
                }

                if(!int.TryParse(ProductPrice, out int price))
                {
                    MessageBox.Show("가격은 숫자로 입력해주세요.");
                    return;
                }

                if(price <= 0)
                {
                    MessageBox.Show("가격은 1 이상이어야 합니다.");
                    return;
                }

                CreateProductRequest request = new CreateProductRequest()
                {
                    Name  = ProductName,
                    Price = price
                };

                bool isSuccess = await _productService.CreateProductAsync(request);

                if(!isSuccess)
                {
                    MessageBox.Show("상품 추가 실패");
                    return;
                }
                MessageBox.Show("상품이 추가되었습니다.");

                ClearInput();
                await LoadProductAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"상품 추가 중 오류 발생\n{ex.Message}");
            }
        }

        public async Task UpdateProductAsync()
        {
            try 
            {
                if(SelectedProduct == null)
                {
                    MessageBox.Show("수정 할 상품을 선택해주세요");
                    return;
                }

                if(string.IsNullOrWhiteSpace(ProductName))
                {
                    MessageBox.Show("상품명을 입력해주세요");
                    return;
                }

                if(!int.TryParse(ProductPrice, out int price))
                {
                    MessageBox.Show("가격은 숫자로 입력해주세요");
                    return;
                }

                if(price <= 0)
                {
                    MessageBox.Show("가격은 1 이상이어야 합니다.");
                    return;
                }

                UpdateProductRequest request = new UpdateProductRequest
                {
                    Name  = ProductName,
                    Price = price
                };
                bool isSuccess = await _productService.UpdateProductAsync(SelectedProduct.Id, request);
                
                if(!isSuccess)
                {
                    MessageBox.Show("상품 수정 실패");
                    return;
                }
                else
                {
                    MessageBox.Show("상품이 수정되었습니다.");
                    return;
                }

                ClearInput();
                await LoadProductAsync();
            }
            catch(Exception ex)
            {
                MessageBox.Show($"상품 수정 중 오류 발생\n{ex.Message}");
            }
        }

        private async Task DeleteProductAsync()
        {
            try
            {
                if(SelectedProduct == null)
                {
                    MessageBox.Show("삭제 할 상품을 선택해주세요");
                    return;
                }

                MessageBoxResult result = MessageBox.Show("정말 삭제하시겠습니까?", "삭제 확인", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes)
                {
                    return;
                }

                bool isSuccess = await _productService.DeleteProductAsync(SelectedProduct.Id);

                if(!isSuccess)
                {
                    MessageBox.Show("상품 삭제 실패");
                    return;
                }

                MessageBox.Show("상품이 삭제되었습니다.");

                ClearInput();
                await LoadProductAsync();
            }
            catch(Exception ex)
            {
                MessageBox.Show($"상품 삭제 중 오류 발생\n{ex.Message}");
            }
        }

        public void ClearInput()
        {
            ProductName   = "";
            ProductPrice  = "";
            SelectedProduct = null;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
