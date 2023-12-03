﻿using System;
using System.Collections.Generic;
using System.Text;
class Program
{
    static void Main(string[] args)
    {
        double startingBudget = 10000;
        OnlineBookstore bookstore = new OnlineBookstore(startingBudget);
        while (true)
        {
            Console.Clear();
            ConsoleUtils.PrintCentered("Welcome to the Online Bookstore");
            ConsoleUtils.PrintCentered("(1) Register User");
            ConsoleUtils.PrintCentered("(2) Login");
            ConsoleUtils.PrintCentered("(3) Display Products");
            ConsoleUtils.PrintCentered("(4) Order Books");
            ConsoleUtils.PrintCentered("(5) View Order History");
            ConsoleUtils.PrintCentered("(6) Sell a Book");
            ConsoleUtils.PrintCentered("(7) Logout");
            ConsoleUtils.PrintCentered("(8) Exit");
            string choice = ConsoleUtils.PromptCenteredInput("Enter your choice: ");

            switch (choice)
            {
                case "1":
                    Console.Clear();
                    bookstore.RegisterUser();
                    ConsoleUtils.PrintCentered("Press any key to the main menu...");
                    Console.ReadKey();
                    break;
                case "2":
                    bookstore.Login();
                    break;
                case "3":
                    Console.Clear();
                    bookstore.DisplayProducts();
                    ConsoleUtils.PrintCentered("Press any key to the main menu...");
                    Console.ReadKey();
                    break;
                case "4":
                    Console.Clear();
                    if (bookstore.CurrentUser.Username != "guest")
                    {
                        OrderBooks(bookstore, bookstore.CurrentUser);
                    }
                    else
                    {
                        ConsoleUtils.PrintCentered("Only registered users can order books. Please log in or register.");
                    }
                    ConsoleUtils.PrintCentered("Press any key to the main menu...");
                    Console.ReadKey();
                    break;
                case "5":
                    Console.Clear();
                    bookstore.DisplayCurrentUserOrderHistory();
                    ConsoleUtils.PrintCentered("Press any key to the main menu...");
                    Console.ReadKey();
                    break;
                case "6":
                    Console.Clear();
                    bookstore.SellBook();
                    ConsoleUtils.PrintCentered("Press any key to the main menu...");
                    Console.ReadKey();
                    break;
                case "7":
                    bookstore.Logout();
                    break;
                case "8":
                    bookstore.SaveUsersToFile();
                    Environment.Exit(0);
                    break;
                case "slsrprt": 
                    Console.Clear();
                    bookstore.DisplaySalesReport();
                    ConsoleUtils.PrintCentered("Press any key to return to the main menu...");
                    Console.ReadKey();
                    break;
                default:
                    ConsoleUtils.PrintCentered("Invalid choice. Please try again.");
                    Console.ReadKey();
                    break;
            }
        }
    }
    static void OrderBooks(OnlineBookstore bookstore, User user)
    {
        ConsoleUtils.PrintCentered("Order Books");
        bookstore.DisplayProducts();

        ShoppingCart cart = new ShoppingCart();

        while (true)
        {
            string title = ConsoleUtils.PromptCenteredInput("Enter the title of the book to add to the cart (or 'done' to finish): ");

            if (title.ToLower() == "done")
            {
                break; 
            }

            Product selectedBook = bookstore.GetProductByTitle(title);

            if (selectedBook != null && selectedBook.Quantity > 0)
            {
                cart.AddItem(selectedBook);
                ConsoleUtils.PrintCentered($"{title} added to the cart.");
            }
            else
            {
                ConsoleUtils.PrintCentered("Book not found or out of stock.");
            }
        }

        if (cart.GetItems().Count > 0)
        {
            Console.Clear();
            double totalCost = cart.CalculateTotalCost();
            ConsoleUtils.PrintCentered($"Total Cost: ${totalCost:F2}");
            string confirm = ConsoleUtils.PromptCenteredInput("Confirm the order (yes/no): ");
            if (confirm == "yes")
            {
               
                foreach (var item in cart.GetItems())
                {
                    Product product = bookstore.GetProductByTitle(item.Title);
                    if (product != null)
                    {
                        product.Quantity--;
                    }
                }

                bookstore.ProcessOrder(user, cart);
                ConsoleUtils.PrintCentered("Order placed successfully.");
            }
            else
            {
                ConsoleUtils.PrintCentered("Order canceled.");
            }
        }
        else
        {
            ConsoleUtils.PrintCentered("No items in the cart.");
        }
    }
}
class ConsoleUtils
{
    public static void PrintCentered(string text)
    {
        int consoleWidth = Console.WindowWidth;
        int textLength = text.Length;
        int spaces = (consoleWidth / 2) + (textLength / 2);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(String.Format("{0," + spaces + "}", text));
    }
    public static string PromptCenteredInput(string prompt)
    {
        int consoleWidth = Console.WindowWidth;
        int promptLength = prompt.Length;
        int leftPadding = (consoleWidth / 2) - (promptLength / 2);

        Console.SetCursorPosition(leftPadding, Console.CursorTop);
        Console.Write(prompt);
        return Console.ReadLine();
    }
}
class Product
{
    public string Title { get; }
    public double Price { get; set; }
    public int Quantity { get; set; }

    public Product(string title, double price, int quantity)
    {
        Title = title;
        Price = price;
        Quantity = quantity;
    }
}
class ShoppingCart
{
    private List<Product> items = new List<Product>();

    public void AddItem(Product item)
    {
        items.Add(item);
    }

    public double CalculateTotalCost()
    {
        double totalCost = 0;

        foreach (var item in items)
        {
            totalCost += item.Price;
        }

        return totalCost;
    }

    public List<Product> GetItems()
    {
        return items;
    }
}
class User
{
    public string Username { get; private set; }
    internal string Password { get; }
    private List<Order> orderHistory = new List<Order>();
    public string GetUserDataForSaving()
    {
        return $"{Username},{Password}"; 
    }
    public User(string username, string password)
    {
        Username = username;
        Password = password;
    }
    public bool ValidatePassword(string inputPassword)
    {
        return Password.Equals(inputPassword);
    }
    public void DisplayOrderHistory()
    {
        Console.WriteLine($"Order history for {Username}:");
        foreach (var order in orderHistory)
        {
            Console.WriteLine($"Order ID: {order.OrderId}, Total Cost: ${order.TotalCost:F2}");
        }
    }

    public void AddOrderToHistory(Order order)
    {
        orderHistory.Add(order);
    }
}
class Order
{
    public int OrderId { get; }
    public List<Product> OrderedItems { get; }

    public double TotalCost
    {
        get
        {
            double totalCost = 0;
            foreach (var item in OrderedItems)
            {
                totalCost += item.Price;
            }
            return totalCost;
        }
    }

    public Order(List<Product> orderedItems)
    {
        OrderId = new Random().Next(1, 1000);
        OrderedItems = new List<Product>(orderedItems);
    }
}
class OnlineBookstore
{
    private List<Product> products = new List<Product>();
    private List<User> users = new List<User>();
    private User currentUser;
    private double budget;
    private const string InventoryFilePath = "C:\\Users\\User\\source\\repos\\Bookstore (-_-)'\\inventory.txt";
    private const string UserFilePath = "C:\\Users\\User\\source\\repos\\Bookstore (-_-)'\\users.txt";
    public User CurrentUser
    {
        get { return currentUser; }
    }
    private void InitializeDefaultBooks()
    {
        AddProduct(new Product("The Great Gatsby", 10.99, 50));
        AddProduct(new Product("To Kill a Mockingbird", 12.99, 40));
        AddProduct(new Product("1984", 9.99, 30));
        AddProduct(new Product("Pride and Prejudice", 11.99, 60));
    }
    public OnlineBookstore(double startingBudget)
    {
        currentUser = new User("guest", "");
        LoadUsersFromFile();
        LoadInventoryFromFile();
        this.budget = startingBudget;
    }
    public void RegisterUser()
    {
        while (true)
        {
            ConsoleUtils.PrintCentered("Register a new user");
            string username = ConsoleUtils.PromptCenteredInput("Enter username: ");
            

            if (IsUsernameExists(username))
            {
                ConsoleUtils.PrintCentered("Username already in use. Please try a different username.");
                continue;
            }

            string password = ConsoleUtils.PromptCenteredInput("Enter password: ");
            User newUser = new User(username, password);
            users.Add(newUser);
            ConsoleUtils.PrintCentered($"User '{username}' registered successfully.");
            break;
        }
        
    }
    public void RegisterUser(User user)
    {
        users.Add(user);
    }
    public void SaveUsersToFile()
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(UserFilePath, false))
            {
                foreach (var user in users)
                {
                    writer.WriteLine(user.GetUserDataForSaving());
                }
            }
        }
        catch (Exception ex)
        {
            ConsoleUtils.PrintCentered($"An error occurred while saving users: {ex.Message}");
        }
    }
    public void SaveInventoryToFile()
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(InventoryFilePath, false))
            {
                foreach (var product in products)
                {
                    writer.WriteLine($"{product.Title},{product.Price},{product.Quantity}");
                }
            }
        }
        catch (Exception ex)
        {
            ConsoleUtils.PrintCentered($"An error occurred while saving inventory: {ex.Message}");
        }
    }
    private void LoadUsersFromFile()
    {
        if (File.Exists(UserFilePath))
        {
            using (StreamReader reader = new StreamReader(UserFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split(',');
                    if (parts.Length == 2)
                    {
                        users.Add(new User(parts[0], parts[1])); 
                    }
                }
            }
        }
    }
    private void LoadInventoryFromFile()
    {
        if (File.Exists(InventoryFilePath))
        {
            products.Clear();

            using (StreamReader reader = new StreamReader(InventoryFilePath))
            {
                bool hasData = false;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    hasData = true;
                    var parts = line.Split(',');
                    if (parts.Length == 3)
                    {
                        string title = parts[0];
                        double price = double.Parse(parts[1]);
                        int quantity = int.Parse(parts[2]);
                        products.Add(new Product(title, price, quantity));
                    }
                }
                if (!hasData)
                {
                    InitializeDefaultBooks();
                }
            }
        }
        else
        {
            InitializeDefaultBooks();
        }
    }
    private bool IsUsernameExists(string username)
    {
        return users.Exists(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
    }
    public void Login()
    {
        LoadUsersFromFile();
        Console.Clear();
        ConsoleUtils.PrintCentered("Login to your account");
        string username = ConsoleUtils.PromptCenteredInput("Enter username: ");
        string password = ConsoleUtils.PromptCenteredInput("Enter password: ");
        User user = users.Find(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        if (user != null && user.ValidatePassword(password))
        {
            
            currentUser = user;
            LogUserLogin(username);
            ConsoleUtils.PrintCentered("Login successful.");
            Console.ReadKey();
        }
        else
        {
            ConsoleUtils.PrintCentered("Invalid username or password.");
            Console.ReadKey();
        }
    }
    private void LogUserLogin(string username)
    {
        string logFilePath = "C:\\Users\\User\\source\\repos\\Bookstore (-_-)'\\user_logins.txt";
        bool fileExists = File.Exists(logFilePath);

        using (StreamWriter sw = new StreamWriter(logFilePath, true))
        {
            if (!fileExists)
            {
                sw.WriteLine("Username,LoginDate,LoginTime");
            }
            sw.WriteLine($"{username},{DateTime.Now.ToShortDateString()},{DateTime.Now.ToLongTimeString()}");
        }
    }
    public void Logout()
    {
        Console.Clear();
        currentUser = new User("guest", ""); 
        ConsoleUtils.PrintCentered("You have been logged out.");
        Console.ReadKey();
    }
    public void AddProduct(Product product)
    {
        products.Add(product);
    }
    public void DisplayProducts()
    {
        StringBuilder tableBuilder = new StringBuilder();
        string header = "| Title                     | Price ($) | Quantity |";
        string separator = new string('-', header.Length);

        tableBuilder.AppendLine(separator);
        tableBuilder.AppendLine(header);
        tableBuilder.AppendLine(separator);

        foreach (var product in products)
        {
            tableBuilder.AppendLine($"| {product.Title,-25} | {product.Price,-9:F2} | {product.Quantity,-8} |");
        }

        tableBuilder.AppendLine(separator);

        PrintCenteredTable(tableBuilder.ToString());
    }
    private void PrintCenteredTable(string table)
    {
        string[] lines = table.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        int consoleWidth = Console.WindowWidth;
        int maxLineLength = lines.Max(line => line.Length);
        int leftPadding = (consoleWidth - maxLineLength) / 2;
        leftPadding = Math.Max(leftPadding, 0); 
        foreach (string line in lines)
        {
            Console.WriteLine(String.Format("{0," + (leftPadding + line.Length) + "}", line));
        }
    }
    public Product GetProductByTitle(string title)
    {
        return products.Find(p => p.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
    }
    public void ProcessOrder(User user, ShoppingCart cart)
    {
        Order order = new Order(cart.GetItems());
        user.AddOrderToHistory(order);
        budget += order.TotalCost;
        WriteOrderToFile(order, user.Username);
        SaveInventoryToFile();
    }
    private void WriteOrderToFile(Order order, string username)
    {
        string filePath = "C:\\Users\\User\\source\\repos\\Bookstore (-_-)'\\sales_report.txt";
        bool fileExists = File.Exists(filePath);

        using (StreamWriter sw = new StreamWriter(filePath, true))
        {
            if (!fileExists)
            {
                sw.WriteLine("OrderID,Username,TotalCost,Date");
            }
            sw.WriteLine($"{order.OrderId},{username},${order.TotalCost:F2},{DateTime.Now}");
        }
    }
    public void DisplayCurrentUserOrderHistory()
    {
        if (currentUser.Username != "guest")
        {
            currentUser.DisplayOrderHistory();
        }
        else
        {
            ConsoleUtils.PrintCentered("You must be a guest, Please press any key to return to the main menu and register/login");
        }
    }
    public void DisplaySalesReport()
    {
        string filePath = "C:\\Users\\User\\source\\repos\\Bookstore (-_-)'\\sales_report.txt"; 
        if (!File.Exists(filePath))
        {
            ConsoleUtils.PrintCentered("Sales report is not available.");
            return;
        }

        string header = "| OrderID | Username    | Total Cost | Date                 |";
        string separator = new string('-', header.Length);

        ConsoleUtils.PrintCentered(separator);
        ConsoleUtils.PrintCentered(header);
        ConsoleUtils.PrintCentered(separator);

        foreach (string line in File.ReadLines(filePath).Skip(1)) 
        {
            var parts = line.Split(',');
            if (parts.Length >= 4) 
            {
                string orderLine = $"| {parts[0],-7} | {parts[1],-11} | ${parts[2],-9} | {parts[3],-19} |";
                ConsoleUtils.PrintCentered(orderLine);
            }
        }

        ConsoleUtils.PrintCentered(separator);
    }
    public void SellBook()
    {
        if (currentUser.Username == "guest")
        {
            ConsoleUtils.PrintCentered("You must be logged in to sell books.");
            return;
        }
        ConsoleUtils.PrintCentered("Sell your book to the bookstore");
        string title = PromptForBookTitle();
        double price = PromptForBookPrice();
        int quantity = PromptForBookQuantity();
        Product existingBook = GetProductByTitle(title);

        ConsoleUtils.PrintCentered($"'{title}' added to the bookstore inventory.");
        if (existingBook != null)
        {
            existingBook.Quantity += quantity;
            existingBook.Price = price; 
            ConsoleUtils.PrintCentered($"Updated quantity of '{title}'.");
        }
        else
        {
            
            if (budget >= price * quantity)
            {
                Product newBook = new Product(title, price, quantity);
                AddProduct(newBook);
                budget -= price * quantity;
                ConsoleUtils.PrintCentered($"'{title}' added to the bookstore inventory.");
            }
            else
            {
                ConsoleUtils.PrintCentered("The store does not have enough budget to buy this book.");
            }
        }
        SaveInventoryToFile();
    }
    private string PromptForBookTitle()
    {
        while (true)
        {
            string title = ConsoleUtils.PromptCenteredInput("Enter book title: ");
            if (!string.IsNullOrWhiteSpace(title))
            {
                return title;
            }
            ConsoleUtils.PrintCentered("Invalid input. Title cannot be empty.");
        }
    }
    private double PromptForBookPrice()
    {
        while (true)
        {
            string input = ConsoleUtils.PromptCenteredInput("Enter book price: ");
            if (double.TryParse(input, out double price) && price > 0)
            {
                return price;
            }
            ConsoleUtils.PrintCentered("Invalid input. Please enter a positive number for the price.");
        }
    }
    private int PromptForBookQuantity()
    {
        while (true)
        {
            string input = ConsoleUtils.PromptCenteredInput("Enter quantity: ");
            if (int.TryParse(input, out int quantity) && quantity > 0)
            {
                return quantity;
            }
            ConsoleUtils.PrintCentered("Invalid input. Please enter a positive integer for the quantity.");
        }
    }
}