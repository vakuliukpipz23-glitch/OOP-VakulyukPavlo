using System;

namespace lab22
{
    // ===================== ПОРУШЕННЯ LSP =====================

    class Rectangle
    {
        public virtual int Width { get; set; }
        public virtual int Height { get; set; }

        public int Area()
        {
            return Width * Height;
        }
    }

    class Square : Rectangle
    {
        public override int Width
        {
            get => base.Width;
            set
            {
                base.Width = value;
                base.Height = value;
            }
        }

        public override int Height
        {
            get => base.Height;
            set
            {
                base.Width = value;
                base.Height = value;
            }
        }
    }

    class LspViolationDemo
    {
        public static void ResizeRectangle(Rectangle rectangle)
        {
            rectangle.Width = 10;
            rectangle.Height = 5;

            Console.WriteLine("Очікувана площа: 50");
            Console.WriteLine($"Фактична площа: {rectangle.Area()}");
        }
    }

    // ===================== ВИПРАВЛЕНЕ РІШЕННЯ =====================

    interface IShape
    {
        int Area();
    }

    class RectangleFixed : IShape
    {
        public int Width { get; }
        public int Height { get; }

        public RectangleFixed(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Area()
        {
            return Width * Height;
        }
    }

    class SquareFixed : IShape
    {
        public int Side { get; }

        public SquareFixed(int side)
        {
            Side = side;
        }

        public int Area()
        {
            return Side * Side;
        }
    }

    class LspCorrectDemo
    {
        public static void PrintArea(IShape shape)
        {
            Console.WriteLine($"Площа фігури: {shape.Area()}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Порушення LSP ===");

            Rectangle rectangle = new Rectangle();
            Square square = new Square();

            Console.WriteLine("\nRectangle:");
            LspViolationDemo.ResizeRectangle(rectangle);

            Console.WriteLine("\nSquare (як Rectangle):");
            LspViolationDemo.ResizeRectangle(square);

            Console.WriteLine("\n=== LSP дотримано ===");

            IShape rectFixed = new RectangleFixed(10, 5);
            IShape squareFixed = new SquareFixed(5);

            LspCorrectDemo.PrintArea(rectFixed);
            LspCorrectDemo.PrintArea(squareFixed);

            Console.ReadLine();
        }
    }
}
