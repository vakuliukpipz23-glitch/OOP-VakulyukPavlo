using System;
using System.Collections.Generic;

// ==========================
// Інтерфейс IArea — оголошує метод для обчислення площі
// ==========================
public interface IArea
{
    double Area();
}

// ==========================
// Абстрактний клас Shape — спільні властивості для всіх фігур
// ==========================
public abstract class Shape : IArea
{
    protected string name;

    public Shape(string name)
    {
        this.name = name;
        Console.WriteLine($"Створено фігуру: {name}");
    }

    public abstract double Area();

    public virtual void ShowInfo()
    {
        Console.WriteLine($"Фігура: {name}, Площа: {Area():F2}");
    }
}

// ==========================
// Клас Circle — реалізація абстрактного класу та інтерфейсу
// ==========================
public class Circle : Shape
{
    private double radius;

    public Circle(double radius) : base("Коло")
    {
        this.radius = radius;
    }

    public override double Area() => Math.PI * radius * radius;

    public override void ShowInfo()
    {
        base.ShowInfo();
        Console.WriteLine($"Радіус: {radius}");
    }
}

// ==========================
// Клас Rectangle — реалізація абстрактного класу та інтерфейсу
// ==========================
public class Rectangle : Shape
{
    private double width;
    private double height;

    public Rectangle(double width, double height) : base("Прямокутник")
    {
        this.width = width;
        this.height = height;
    }

    public override double Area() => width * height;

    public override void ShowInfo()
    {
        base.ShowInfo();
        Console.WriteLine($"Ширина: {width}, Висота: {height}");
    }
}

// ==========================
// Клас Canvas — демонструє композицію: містить список фігур
// ==========================
public class Canvas
{
    private List<Shape> shapes; // Агрегація/композиція

    public Canvas()
    {
        shapes = new List<Shape>();
    }

    public void AddShape(Shape shape)
    {
        shapes.Add(shape);
    }

    public void ShowAll()
    {
        Console.WriteLine("\n--- Інформація про всі фігури на Canvas ---");
        foreach (var shape in shapes)
        {
            shape.ShowInfo();
            Console.WriteLine();
        }
    }

    public double TotalArea()
    {
        double total = 0;
        foreach (var shape in shapes)
            total += shape.Area();
        return total;
    }

    public Shape MinAreaShape()
    {
        if (shapes.Count == 0) return null;
        Shape minShape = shapes[0];
        foreach (var shape in shapes)
            if (shape.Area() < minShape.Area())
                minShape = shape;
        return minShape;
    }

    public Shape MaxAreaShape()
    {
        if (shapes.Count == 0) return null;
        Shape maxShape = shapes[0];
        foreach (var shape in shapes)
            if (shape.Area() > maxShape.Area())
                maxShape = shape;
        return maxShape;
    }
}

// ==========================
// Головна програма
// ==========================
class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("=== Лабораторна робота №4: Абстракції та інтерфейси ===\n");

        Canvas canvas = new Canvas();

        // Додаємо фігури
        canvas.AddShape(new Circle(5));
        canvas.AddShape(new Rectangle(4, 6));
        canvas.AddShape(new Circle(2.5));
        canvas.AddShape(new Rectangle(3, 7));

        // Показати всі фігури
        canvas.ShowAll();

        // Загальна площа
        Console.WriteLine($"Сумарна площа всіх фігур: {canvas.TotalArea():F2}");

        // Мінімальна та максимальна площа
        Shape minShape = canvas.MinAreaShape();
        Shape maxShape = canvas.MaxAreaShape();

        Console.WriteLine("\nФігура з мінімальною площею:");
        minShape?.ShowInfo();

        Console.WriteLine("\nФігура з максимальною площею:");
        maxShape?.ShowInfo();

        Console.WriteLine("\n--- Кінець роботи програми ---");
    }
}
