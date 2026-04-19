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
        private Product _selectProduct;

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

        public Product? SelectProduct
        {
            get => _selectProduct;
            set 
            {
                _selectProduct = value;
                OnPropertyChanged();

                if (_selectProduct != null)
                {
                    ProductName  = _selectProduct.Name;
                    ProductPrice = _selectProduct.Price.ToString();
                }
            }
        }

        public ICommand LoadProductCommand   { get; }
        public ICommand CreateProductCommand { get; }
        public ICommand UpdateProductCommand { get; }
        public ICommand DeleteProductCommand { get; }

        public MainViewModel()
        {
            _productService = new ProductService();

            LoadProductCommand   = new RelayCommand(async _ => await LoadProductAsync());
            CreateProductCommand = new RelayCommand(async _ => await CreateProductAsync());
            UpdateProductCommand = new RelayCommand(async _ => await UpdateProductAsync());
            DeleteProductCommand = new RelayCommand(async _ => await DeleteProductAsync());
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
                if(string.IsNullOrWhiteSpace(ProductName))
                {
                    MessageBox.Show("상품명을 입력해주세요.");
                    return;
                }

                if(!int.TryParse(ProductPrice, out int price))
                {
                    MessageBox.Show("가격은 숫자로 입력해주세요.");
                }

                if(price <= 0)
                {
                    MessageBox.Show("가격은 1 이상이어야 합니다.");
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
                if(SelectProduct == null)
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

                bool isSuccess = await _productService.UpdateProductAsync(SelectProduct.Id, request);
                
                if(!isSuccess)
                {
                    MessageBox.Show("상품 수정 실패");
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
                if(SelectProduct == null)
                {
                    MessageBox.Show("삭제 할 상품을 선택해주세요");
                    return;
                }

                MessageBoxResult result = MessageBox.Show("정말 삭제하시겠습니까?", "삭제 확인", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes)
                {
                    return;
                }

                bool isSuccess = await _productService.DeleteProductAsync(SelectProduct.Id);

                if(isSuccess)
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
            SelectProduct = null;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
