using System;
using System.Collections.Generic;

namespace Lab3Inheritance
{
    // =========================
    // Базовий клас Shape
    // =========================
    public abstract class Shape
    {
        protected string name;

        public Shape(string name)
        {
            this.name = name;
            Console.WriteLine($"Створено фігуру: {name}");
        }

        public abstract double Area();
        public abstract double Perimeter();

        public virtual void ShowInfo()
        {
            Console.WriteLine($"Фігура: {name}");
        }

        ~Shape()
        {
            Console.WriteLine($"Об'єкт {name} знищено.");
        }
    }

    // =========================
    // Похідний клас Circle
    // =========================
    public class Circle : Shape
    {
        private double radius;

        public Circle(double radius) : base("Коло")
        {
            this.radius = radius;
        }

        public override double Area() => Math.PI * radius * radius;
        public override double Perimeter() => 2 * Math.PI * radius;

        public override void ShowInfo()
        {
            base.ShowInfo();
            Console.WriteLine($"Радіус: {radius}, Площа: {Area():F2}, Периметр: {Perimeter():F2}");
        }
    }

    // =========================
    // Похідний клас Rectangle
    // =========================
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
        public override double Perimeter() => 2 * (width + height);

        public override void ShowInfo()
        {
            base.ShowInfo();
            Console.WriteLine($"Ширина: {width}, Висота: {height}, Площа: {Area():F2}, Периметр: {Perimeter():F2}");
        }
    }

    // =========================
    // Головна програма
    // =========================
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Лабораторна робота №3: Наслідування ===\n");

            // Колекція базового типу (поліморфізм)
            List<Shape> shapes = new List<Shape>
            {
                new Circle(5),
                new Rectangle(4, 6),
                new Circle(2.5),
                new Rectangle(3, 7)
            };

            // Виведення інформації про всі фігури
            Console.WriteLine("\n--- Інформація про фігури ---");
            foreach (var shape in shapes)
            {
                shape.ShowInfo();
                Console.WriteLine();
            }

            // Знаходження фігури з найбільшою площею
            Shape maxAreaShape = shapes[0];
            foreach (var shape in shapes)
            {
                if (shape.Area() > maxAreaShape.Area())
                    maxAreaShape = shape;
            }

            Console.WriteLine($"Фігура з найбільшою площею:");
            maxAreaShape.ShowInfo();

            Console.WriteLine("\n--- Кінець роботи програми ---");
        }
    }
}