using System;
using System.Collections.Generic;
using System.Linq;

// ===== КЛАС PRODUCT =====
class Product
{
    public string Name { get; set; }
    public double Price { get; set; }
    public string Category { get; set; }

    public Product(string name, double price, string category)
    {
        Name = name;
        Price = price;
        Category = category;
    }

    public override string ToString()
    {
        return $"{Name} | {Category} | {Price} грн";
    }
}


// ====== ДЕЛЕГАТ КОРИСТУВАЧА ======
delegate double Operation(double x, double y);   // делегат для арифметичних операцій


// ======= Виправлений клас ProgramLab6 =======
class ProgramLab6
{
    static void Main()
    {
        Console.WriteLine("=== Lab 6: Delegates, Lambda, Func, Action, Predicate ===\n");

        // =============================== 
        // 1. Приклад використання власного делегата
        // ===============================
        Operation add = delegate (double a, double b)   // Анонімний метод
        {
            return a + b;
        };

        Operation multiply = (x, y) => x * y;           // Лямбда-вираз

        Console.WriteLine("Делегати:");
        Console.WriteLine($"add(4, 6) = {add(4, 6)}");
        Console.WriteLine($"multiply(3, 7) = {multiply(3, 7)}\n");


        // =============================== 
        // 2. Список продуктів
        // ===============================
        List<Product> products = new List<Product>()
        {
            new Product("Хліб", 25, "Food"),
            new Product("Молоко", 38, "Food"),
            new Product("Телефон", 12000, "Electronics"),
            new Product("Навушники", 800, "Electronics"),
            new Product("Цукерки", 90, "Food")
        };


        // =============================== 
        // 3. Predicate — перевірка умови
        // ===============================
        Predicate<Product> isExpensive = p => p.Price > 1000;
        var expensiveProducts = products.FindAll(isExpensive);

        Console.WriteLine("Товари дорожчі за 1000 грн:");
        expensiveProducts.ForEach(p => Console.WriteLine(p));
        Console.WriteLine();


        // =============================== 
        // 4. Func — повертає значення 
        // ===============================
        Func<List<Product>, double> avgPrice = list =>
            list.Average(p => p.Price);

        Console.WriteLine($"Середня ціна товарів: {avgPrice(products):F2} грн\n");


        // =============================== 
        // 5. Action — нічого не повертає
        // ===============================
        Action<Product> print = item => Console.WriteLine(item);

        Console.WriteLine("Вивід усіх товарів:");
        products.ForEach(print);
        Console.WriteLine();


        // =============================== 
        // 6. LINQ — фільтрація, сортування, вибірка
        // ===============================

        // Фільтрація за ціною
        var filtered = products.Where(p => p.Price < 100);

        Console.WriteLine("Товари дешевші ніж 100 грн:");
        foreach (var p in filtered)
            Console.WriteLine(p);
        Console.WriteLine();


        // Сортування
        var sorted = products.OrderBy(p => p.Price);
        Console.WriteLine("Сортування за ціною (зростання):");
        foreach (var p in sorted)
            Console.WriteLine(p);
        Console.WriteLine();


        // Найдорожчий товар
        var maxProduct = products.OrderByDescending(p => p.Price).First();
        Console.WriteLine($"Найдорожчий товар: {maxProduct}\n");


        // Обчислення суми через Aggregate
        double totalCost = products.Aggregate(0.0, (sum, item) => sum + item.Price);
        Console.WriteLine($"Загальна вартість усіх товарів: {totalCost} грн\n");

        Console.WriteLine("=== Кінець роботи ===");
    }
}
