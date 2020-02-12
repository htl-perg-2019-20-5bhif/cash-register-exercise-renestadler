using CashRegister.Shared;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace CashRegister.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        HttpClient Client;

        public ObservableCollection<Product> Products { get; set; }
        public ObservableCollection<ReceiptLine> ShoppingBasket { get; set; }

        public DelegateCommand<int?> AddToBasketCommand { get; }


        private double TotalPriceValue;
        public double TotalPrice
        {
            get => TotalPriceValue;
            set
            {
                TotalPriceValue = value;
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public event PropertyChangedEventHandler PropertyChanged;


        public MainWindow()
        {
            InitializeComponent();

            Client = new HttpClient();
            Client.BaseAddress = new Uri("http://localhost:5000/api/");

            Products = new ObservableCollection<Product>();
            ShoppingBasket = new ObservableCollection<ReceiptLine>();
            GetProducts();

            AddToBasketCommand = new DelegateCommand<int?>(OnAddToBasket);
            this.DataContext = this;
        }
        public async void GetProducts()
        {
            JsonSerializer.Deserialize<IEnumerable<Product>>(
                await (
                await Client.GetAsync("Products")).Content.ReadAsStringAsync(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })
                .ToList().ForEach(p => Products.Add(p));
        }

        private void OnAddToBasket(int? productId)
        {
            var product = Products.First(p => p.ProductId == productId);

            var basketItem = ShoppingBasket.FirstOrDefault(p => p.Product.ProductId == productId);
            if (basketItem != null)
            {
                basketItem.Amount++;
                basketItem.TotalPrice = Math.Round(product.UnitPrice * basketItem.Amount, 2);
                myGrid.Items.Refresh();
            }
            else
            {
                ShoppingBasket.Add(new ReceiptLine
                {
                    Product = product,
                    Amount = 1,
                    TotalPrice = product.UnitPrice
                });
            }

            RecalculateTotal();
        }

        private void RecalculateTotal()
        {
            var total = 0.0;
            foreach (var item in ShoppingBasket)
            {
                total += item.TotalPrice;
            }
            TotalPrice = Math.Round(total, 2);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var content = new StringContent(JsonSerializer.Serialize(ShoppingBasket), Encoding.UTF8, "application/json");
            await Client.PostAsync("ReceiptLines/All", content);
            ShoppingBasket.ToList().ForEach(r => ShoppingBasket.Remove(r));
            TotalPrice = 0;
        }
    }
}
