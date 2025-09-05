using System;

namespace lab1
{
    public class Figure
    {
        
        private double area;

       
        public double Area
        {
            get { return area; }
            set { area = value; }
        }

        
        public Figure(double area)
        {
            this.area = area;
            Console.WriteLine("Конструктор Figure викликаний");
        }


        ~Figure()
        {
            Console.WriteLine("Деструктор Figure викликаний");
        }


        public string GetFigure()
        {
            return $"Фігура з площею {area}";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {

            Figure f = new Figure(25.5);

            Console.WriteLine(f.GetFigure());

            f.Area = 40;
            Console.WriteLine(f.GetFigure());
        }
    }
}
