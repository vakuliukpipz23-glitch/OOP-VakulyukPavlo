using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace IndependentWork12
{
    internal class Program
    {

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;


            int[] sizes = { 1_000_000, 5_000_000, 10_000_000 };

            foreach (int size in sizes)
            {
                Console.WriteLine(new string('-', 70));
                Console.WriteLine($"ТЕСТ ПРОДУКТИВНОСТІ. Розмір колекції: {size:N0} елементів");
                RunPerformanceTest(size);
            }

            Console.WriteLine(new string('-', 70));
            Console.WriteLine("ДОСЛІДЖЕННЯ ПОБІЧНИХ ЕФЕКТІВ / БЕЗПЕКИ:");
            DemonstrateSideEffects();

            Console.WriteLine(new string('-', 70));
            Console.WriteLine("Тести завершено. Натисніть будь-яку клавішу, щоб вийти...");
            Console.ReadKey();
        }

        /// <summary>

        /// </summary>
        static List<int> GenerateRandomList(int count, int minValue = 1, int maxValue = 10_000_000)
        {
            var rnd = new Random();
            var data = new List<int>(count);
            for (int i = 0; i < count; i++)
            {
                data.Add(rnd.Next(minValue, maxValue));
            }
            return data;
        }

        /// <summary>

        /// </summary>
        static bool IsPrime(int n)
        {
            if (n < 2) return false;
            if (n == 2) return true;
            if (n % 2 == 0) return false;

            int limit = (int)Math.Sqrt(n);
            for (int i = 3; i <= limit; i += 2)
            {
                if (n % i == 0) return false;
            }
            return true;
        }

        /// <summary>
      
        /// </summary>
        static void RunPerformanceTest(int count)
        {
            var data = GenerateRandomList(count);


            IsPrime(17);


            var sw = new Stopwatch();


            sw.Start();
            var linqResult = data
                .Where(n => IsPrime(n))
                .Select(n => Math.Sqrt(n))
                .ToList();
            sw.Stop();

            long linqMs = sw.ElapsedMilliseconds;
            Console.WriteLine($"LINQ:   знайдено {linqResult.Count:N0} простих чисел, час = {linqMs} мс");


            sw.Reset();
            sw.Start();
            var plinqResult = data
                .AsParallel()
                .WithDegreeOfParallelism(Environment.ProcessorCount) 
                .Where(n => IsPrime(n))
                .Select(n => Math.Sqrt(n))
                .ToList();
            sw.Stop();

            long plinqMs = sw.ElapsedMilliseconds;
            Console.WriteLine($"PLINQ:  знайдено {plinqResult.Count:N0} простих чисел, час = {plinqMs} мс");


            Console.WriteLine($"Результати однакові за кількістю?  {(linqResult.Count == plinqResult.Count ? "ТАК" : "НІ")}");
        }

        /// <summary>

        /// </summary>
        static void DemonstrateSideEffects()
        {
            var data = GenerateRandomList(1_000_000, 1, 100); 


            int unsafeSum = 0;


            data.AsParallel().ForAll(n =>
            {

                unsafeSum += n; 
            });


            long correctSum = data.Sum(n => (long)n);

            Console.WriteLine($"Правильна сума (послідовно, LINQ):   {correctSum}");
            Console.WriteLine($"Небезпечна сума (PLINQ без lock):     {unsafeSum}");
            Console.WriteLine("Можна побачити, що значення часто НЕ співпадають або змінюються від запуску до запуску.");


            int safeSumWithLock = 0;
            object locker = new object();

            data.AsParallel().ForAll(n =>
            {
                lock (locker)
                {
                    safeSumWithLock += n; 
                }
            });

            Console.WriteLine($"Безпечна сума (PLINQ з lock):         {safeSumWithLock}");


            int safeSumInterlocked = 0;

            data.AsParallel().ForAll(n =>
            {
                Interlocked.Add(ref safeSumInterlocked, n);
            });

            Console.WriteLine($"Безпечна сума (PLINQ Interlocked):    {safeSumInterlocked}");

            Console.WriteLine();
            Console.WriteLine("Висновок по побічних ефектах:");
            Console.WriteLine("- При зміні спільних змінних у PLINQ без синхронізації результат може бути некоректним.");
            Console.WriteLine("- Потрібно використовувати lock, Interlocked, потокобезпечні колекції або уникати побічних ефектів.");
        }
    }
}
