using System;

namespace OOP_Lab2
{

    public class Polynomial
    {

        private double[] coefficients;

        public Polynomial(double[] coeffs)
        {
            coefficients = new double[coeffs.Length];
            for (int i = 0; i < coeffs.Length; i++)
                coefficients[i] = coeffs[i];
        }

        public int Degree => coefficients.Length - 1;

        public double this[int index]
        {
            get
            {
                if (index < 0 || index >= coefficients.Length)
                    throw new IndexOutOfRangeException("Неправильний індекс!");
                return coefficients[index];
            }
            set
            {
                if (index < 0 || index >= coefficients.Length)
                    throw new IndexOutOfRangeException("Неправильний індекс!");
                coefficients[index] = value;
            }
        }

        public static Polynomial operator +(Polynomial p1, Polynomial p2)
        {
            int maxLength = Math.Max(p1.coefficients.Length, p2.coefficients.Length);
            double[] resultCoeffs = new double[maxLength];

            for (int i = 0; i < maxLength; i++)
            {
                double c1 = (i < p1.coefficients.Length) ? p1.coefficients[i] : 0;
                double c2 = (i < p2.coefficients.Length) ? p2.coefficients[i] : 0;
                resultCoeffs[i] = c1 + c2;
            }

            return new Polynomial(resultCoeffs);
        }

        public override string ToString()
        {
            string result = "";
            for (int i = coefficients.Length - 1; i >= 0; i--)
            {
                if (coefficients[i] == 0) continue;
                if (result != "") result += " + ";

                if (i == 0)
                    result += coefficients[i];
                else if (i == 1)
                    result += $"{coefficients[i]}x";
                else
                    result += $"{coefficients[i]}x^{i}";
            }
            return result == "" ? "0" : result;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {

            Polynomial p1 = new Polynomial(new double[] { 1, 2, 3 }); // 1 + 2x + 3x^2
            Polynomial p2 = new Polynomial(new double[] { 4, 5 });    // 4 + 5x

            Console.WriteLine("Поліном p1: " + p1);
            Console.WriteLine("Поліном p2: " + p2);

            Console.WriteLine("Коефіцієнт при x^2 у p1: " + p1[2]);

            Polynomial sum = p1 + p2;
            Console.WriteLine("Сума p1 + p2: " + sum);

            p1[0] = 10;
            Console.WriteLine("Новий p1: " + p1);
        }
    }
}
