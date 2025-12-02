using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace IndependentWork12
{
    internal class Program
    {
        /*
         * ЗВІТ (коротко, у вигляді коментарів)
         *
         * ТЕМА: PLINQ: дослідження продуктивності та безпеки
         *
         * Експерименти з продуктивності:
         * - Колекції: 1 000 000; 5 000 000; 10 000 000 елементів (List<int>).
         * - Дані: випадкові цілі числа у діапазоні [1; 10 000 000].
         * - Обчислювальна операція:
         *      1) Перевірка числа на простоту (IsPrime).
         *      2) Додатково – обчислення Math.Sqrt(n) для простих чисел.
         * - Порівнювали:
         *      - LINQ: data.Where(...).Select(...).ToList()
         *      - PLINQ: data.AsParallel().Where(...).Select(...).ToList()
         * - Час вимірювали через System.Diagnostics.Stopwatch.
         *
         * Висновок (приклад, ПЕРЕПИШИ під свої реальні результати):
         * - На великих колекціях (5M–10M) з важкими обчисленнями PLINQ був помітно швидший,
         *   тому що навантаження розподіляється між кількома ядрами процесора.
         * - На невеликих колекціях різниця або мінімальна, або PLINQ навіть повільніший,
         *   бо є накладні витрати на створення та керування потоками.
         *
         * Побічні ефекти:
         * - Продемонстровано сценарій, коли з PLINQ ми змінюємо спільну змінну (sum)
         *   без синхронізації — результат виявляється некоректним (сума "гуляє").
         * - Проблему виправлено за допомогою lock та за допомогою Interlocked.Add.
         *
         * Висновки щодо PLINQ:
         * - PLINQ доцільно використовувати для:
         *      - великих колекцій;
         *      - важких обчислень для кожного елемента;
         *      - сценаріїв без побічних ефектів (чисті функції).
         * - Важливо враховувати:
         *      - накладні витрати паралелізму;
         *      - потокобезпечність при роботі зі спільними ресурсами;
         *      - можливу зміну порядку елементів (якщо порядок важливий — AsOrdered()).
         */

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Розміри колекцій для тестів
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
        /// Генерує список випадкових чисел.
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
        /// Обчислювально інтенсивна операція: перевірка на просте число.
        /// (дуже неоптимальний, але спеціально важкий алгоритм)
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
        /// Запускає порівняння LINQ vs PLINQ для заданого розміру колекції
        /// з використанням обчислювально інтенсивної операції.
        /// </summary>
        static void RunPerformanceTest(int count)
        {
            var data = GenerateRandomList(count);

            // Трошки "розігріємо" JIT-компілятор (warm-up),
            // щоб перший виклик не спотворював результати
            IsPrime(17);

            // ОПЕРАЦІЯ:
            // 1) Фільтрація простих чисел
            // 2) Обчислення квадратного кореня для кожного простого числа

            var sw = new Stopwatch();

            // --------- ЗВИЧАЙНИЙ LINQ ---------
            sw.Start();
            var linqResult = data
                .Where(n => IsPrime(n))
                .Select(n => Math.Sqrt(n))
                .ToList();
            sw.Stop();

            long linqMs = sw.ElapsedMilliseconds;
            Console.WriteLine($"LINQ:   знайдено {linqResult.Count:N0} простих чисел, час = {linqMs} мс");

            // --------- PLINQ ---------
            sw.Reset();
            sw.Start();
            var plinqResult = data
                .AsParallel()
                .WithDegreeOfParallelism(Environment.ProcessorCount) // можна прибрати за бажанням
                .Where(n => IsPrime(n))
                .Select(n => Math.Sqrt(n))
                .ToList();
            sw.Stop();

            long plinqMs = sw.ElapsedMilliseconds;
            Console.WriteLine($"PLINQ:  знайдено {plinqResult.Count:N0} простих чисел, час = {plinqMs} мс");

            // Для коректності перевіримо, що результати однакові за кількістю
            Console.WriteLine($"Результати однакові за кількістю?  {(linqResult.Count == plinqResult.Count ? "ТАК" : "НІ")}");
        }

        /// <summary>
        /// Демонструє побічні ефекти при паралельній обробці
        /// та способи їх виправлення.
        /// </summary>
        static void DemonstrateSideEffects()
        {
            var data = GenerateRandomList(1_000_000, 1, 100); // невеликий діапазон, але багато елементів

            // ---- НЕПРАВИЛЬНИЙ СЦЕНАРІЙ (НЕПОТОКОБЕЗПЕЧНО) ----
            int unsafeSum = 0;

            // паралельно додаємо до спільної змінної без синхронізації
            data.AsParallel().ForAll(n =>
            {
                // побічний ефект: зміна спільної змінної
                unsafeSum += n; // НЕПОТОКОБЕЗПЕЧНО!
            });

            // Правильна послідовна сума (еталон)
            long correctSum = data.Sum(n => (long)n);

            Console.WriteLine($"Правильна сума (послідовно, LINQ):   {correctSum}");
            Console.WriteLine($"Небезпечна сума (PLINQ без lock):     {unsafeSum}");
            Console.WriteLine("Можна побачити, що значення часто НЕ співпадають або змінюються від запуску до запуску.");

            // ---- ВИПРАВЛЕННЯ #1: lock ----
            int safeSumWithLock = 0;
            object locker = new object();

            data.AsParallel().ForAll(n =>
            {
                lock (locker)
                {
                    safeSumWithLock += n; // тепер потокобезпечно
                }
            });

            Console.WriteLine($"Безпечна сума (PLINQ з lock):         {safeSumWithLock}");

            // ---- ВИПРАВЛЕННЯ #2: Interlocked.Add ----
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
