using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TimerbaevUkrashenie
{
    /// <summary>
    /// Логика взаимодействия для ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Page
    {
        public ProductPage()
        {
            InitializeComponent();
            var currentProduct = TimerbaevTradeEntities.GetContext().Product.ToList();
            ProductListView.ItemsSource = currentProduct;
            SortProduct.SelectedIndex = 0;
            FilterProduct.SelectedIndex = 0;
        }
        private bool isUpdating = false;

        private void UpdateProduct()
        {
            var context = TimerbaevTradeEntities.GetContext();
            var currentProduct = context.Product.AsQueryable();

            // Поиск по наименованию товара
            if (!string.IsNullOrWhiteSpace(SearchProduct.Text))
            {
                currentProduct = currentProduct.Where(p => p.ProductName.ToLower().Contains(SearchProduct.Text.ToLower()));
            }

            // Фильтрация по размеру скидки
            if (FilterProduct.SelectedIndex > 0) // Если выбран не "Все диапазоны"
            {
                switch (FilterProduct.SelectedIndex)
                {
                    case 1: // 0-9.99%
                        currentProduct = currentProduct.Where(p => p.ProductDiscountAmount < 10);
                        break;
                    case 2: // 10-14.99%
                        currentProduct = currentProduct.Where(p => p.ProductDiscountAmount >= 10 && p.ProductDiscountAmount < 15);
                        break;
                    case 3: // 15% и более
                        currentProduct = currentProduct.Where(p => p.ProductDiscountAmount >= 15);
                        break;
                }
            }

            // Сортировка
            switch (SortProduct.SelectedIndex)
            {
                case 1: // По возрастанию
                    currentProduct = currentProduct.OrderBy(p => p.ProductCost);
                    break;
                case 2: // По убыванию
                    currentProduct = currentProduct.OrderByDescending(p => p.ProductCost);
                    break;
            }


            ProductListView.ItemsSource = currentProduct.ToList();

            UpdateCountDisplay(currentProduct.Count(), context.Product.Count());
        }

        private void SearchProduct_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateProduct();
        }

        private void SortProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProduct();
        }

        private void FilterProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProduct();
        }

        private void UpdateCountDisplay(int filteredCount, int totalCount)
        {
            CountFiltered.Text = filteredCount.ToString();
            CountTotal.Text = totalCount.ToString();
        }

    }
}
