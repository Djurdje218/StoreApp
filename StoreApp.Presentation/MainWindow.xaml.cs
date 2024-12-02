using AutoMapper;
using BLL.AutoMapper;
using BLL.DTO;
using BLL.Services;
using DAL;
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

namespace StoreApp
{
    public partial class MainWindow : Window
    {
        private readonly StoreService _storeService;
        private readonly ProductService _productService;

        public MainWindow(StoreService storeService, ProductService productService)
        {
            this.FontFamily = new FontFamily("Segoe UI");
            InitializeComponent();
            _storeService = storeService;
            _productService = productService;
    

            LoadStores();
            LoadProducts();
        }



        // Helper to load all stores into the ListBox
        private async void LoadStores()
        {
            StoresListBox.Items.Clear();
            var stores = await _storeService.GetAllStoresAsync();
            foreach (var store in stores)
            {
                StoresListBox.Items.Add($"{store.Code}: {store.Name}  -  {store.Address}");
            }
        }

        // Helper to load all products into the ListBox
        private async void LoadProducts()
        {
            ProductsListBox.Items.Clear();
            var products = await _productService.GetAllProductsAsync();
            foreach (var product in products)
            {
                ProductsListBox.Items.Add($"id({product.id}) - {product.Name} (Store: {product.StoreCode}, Price: {product.Price}, Qty: {product.Quantity})");
            }
        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && textBox.Text == (string)textBox.Tag) // Check if the text is the placeholder
            {
                textBox.Text = ""; // Clear the placeholder text
                textBox.Foreground = new SolidColorBrush(Colors.Black); // Set text color to black
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrEmpty(textBox.Text)) // if no text is entered
            {
                textBox.Text = (string)textBox.Tag; // Restore placeholder text
                textBox.Foreground = new SolidColorBrush(Colors.Gray); // Set text color to gray 
            }
        }
        private async void AddStoreButton_Click(object sender, RoutedEventArgs e)
        {
            var storeName = StoreNameTextBox.Text.Trim();
            var storeCode = StoreCodeTextBox.Text.Trim();
            var storeAddress = StoreAddressTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(storeName) || string.IsNullOrWhiteSpace(storeCode) || StoreNameTextBox.Text == StoreNameTextBox.Tag.ToString() || StoreCodeTextBox.Text == StoreCodeTextBox.Tag.ToString())
            {
                MessageBox.Show("Store Name and Code are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var storeDto = new StoreDto { Name = storeName, Code = int.Parse(storeCode), Address = storeAddress };
                await _storeService.AddStoreAsync(storeDto);
                MessageBox.Show($"Store '{storeName}' added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadStores();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }


            
        }

        // Event handler for adding a new product
        private async void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            var productId = ProductIdTextBox.Text.Trim();
            var productName = ProductNameTextBox.Text.Trim();
            var storeCodeText = ProductStoreCodeTextBox.Text.Trim();
            var quantityText = ProductQuantityTextBox.Text.Trim();
            var priceText = ProductPriceTextBox.Text.Trim();

            if ( string.IsNullOrWhiteSpace(productName) || string.IsNullOrWhiteSpace(storeCodeText) ||
                string.IsNullOrWhiteSpace(quantityText) || string.IsNullOrWhiteSpace(priceText) ||
                ProductNameTextBox.Text == ProductNameTextBox.Tag.ToString() || string.IsNullOrWhiteSpace(productId) )
            {
                MessageBox.Show("All fields are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(storeCodeText, out var storeCode) || storeCode < 0)
            {
                MessageBox.Show("Store Code must be a valid non-negative number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(quantityText, out var quantity) || quantity < 0)
            {
                MessageBox.Show("Quantity must be a valid non-negative number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(priceText, out var price) || price < 0)
            {
                MessageBox.Show("Price must be a valid non-negative number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(productId, out var ID) || ID < 0)
            {
                MessageBox.Show("Product ID must be a valid non-negative number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            try
            {
                var productDto = new ProductDto
                {
                    id = ID,
                    Name = productName,
                    StoreCode = storeCode,
                    Quantity = quantity,
                    Price = price
                };

                await _productService.AddProductAsync(productDto);

                MessageBox.Show($"Product '{productName}' added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadProducts();
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Event handler for finding cheapest store for product
        private async void FindCheapestStoreButton_Click(object sender, RoutedEventArgs e)
        {
            var productName = QueryProductNameTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(productName) || QueryProductNameTextBox.Text == QueryProductNameTextBox.Tag.ToString())
            {
                MessageBox.Show("Product Name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

         

            var cheapestProduct = await _productService.FindCheapestStoreForProductAsync(productName);


  

            if (cheapestProduct == null)
            {
                MessageBox.Show($"No stores found for product '{productName}'.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                var cheapestStore = await _storeService.GetStoreByCodeAsync(cheapestProduct.StoreCode);
                QueryResultsListBox.Items.Clear();
                QueryResultsListBox.Items.Add($"Cheapest store for '{productName}': {cheapestStore.Code} - {cheapestStore.Name} -> price {cheapestProduct.Price}");
            }
        }

        // Event handler for finding products purchasable within budget
        private async void FindPurchasableProductsButton_Click(object sender, RoutedEventArgs e)
        {
            var storeCode = QueryStoreCodeTextBox.Text.Trim();
            var budgetText = BudgetTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(storeCode) || string.IsNullOrWhiteSpace(budgetText) || 
                QueryStoreCodeTextBox.Text == QueryStoreCodeTextBox.Tag.ToString() || BudgetTextBox.Text == BudgetTextBox.Tag.ToString() )
            {
                MessageBox.Show("Store Code and Budget are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(budgetText, out var budget) || budget < 0)
            {
                MessageBox.Show("Budget must be a valid non-negative number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var products = await _productService.FindPurchasableProductsAsync(int.Parse(storeCode), budget);

            QueryResultsListBox.Items.Clear();

            if (products.Count() == 0)
            {
                QueryResultsListBox.Items.Add("No products found within the given budget.");
            }
            else
            {
                foreach (var product in products)
                {
                    QueryResultsListBox.Items.Add($"{product.Name} - Price: {product.Price}, Qty: {product.Quantity}");
                }
            }
        }

        private async void RestockProductsButton_Click(object sender, RoutedEventArgs e)
        {

            var productName = ProductNameTextBox.Text.Trim();
            var storeCodeText = ProductStoreCodeTextBox.Text.Trim();
            var quantityText = ProductQuantityTextBox.Text.Trim();
            var priceText = ProductPriceTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(productName) || string.IsNullOrWhiteSpace(storeCodeText) ||
                string.IsNullOrWhiteSpace(quantityText) || string.IsNullOrWhiteSpace(priceText) ||
                ProductNameTextBox.Text == ProductNameTextBox.Tag.ToString())
            {
                MessageBox.Show("All fields are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(storeCodeText, out var storeCode) || storeCode < 0)
            {
                MessageBox.Show("Store Code must be a valid non-negative number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(quantityText, out var quantity) || quantity < 0)
            {
                MessageBox.Show("Quantity must be a valid non-negative number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(priceText, out var price) || price < 0)
            {
                MessageBox.Show("Price must be a valid non-negative number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var productUpdates = new Dictionary<string, (int quantity, decimal price)>
                {
                { productName, (quantity, price) }
                };

                await _productService.RestockProductsAsync(storeCode, productUpdates);

                MessageBox.Show("Products restocked successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while restocking products: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            LoadProducts();

        }

    }
}
