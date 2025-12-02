using System;
using System.Net.Http;
using System.Threading;
using Polly;
using Polly.CircuitBreaker;
using Polly.Timeout;

namespace IndependentWork13
{
    internal class Program
    {
        private static int _apiCallAttempts;
        private static int _dbCallAttempts;
        private static int _reportAttempts;

        static void Main(string[] args)
        {
            Console.WriteLine("IndependentWork13 – Кейси Polly (Retry, CircuitBreaker, Timeout)\n");

            Scenario1_ExternalApiWithRetry();
            Separator();

            Scenario2_DatabaseWithRetryAndCircuitBreaker();
            Separator();

            Scenario3_LongRunningOperationWithTimeoutAndFallback();

            Console.WriteLine("\n--- Загальні висновки ---");
            Console.WriteLine("Polly дозволяє централізовано задавати політики відмовостійкості ");
            Console.WriteLine("(Retry, Circuit Breaker, Timeout, Fallback) і повторно використовувати їх.");
            Console.WriteLine("Це підвищує стабільність і керованість .NET-застосунків.\n");
        }

        static void Separator()
        {
            Console.WriteLine("\n====================================\n");
        }


        private static void Scenario1_ExternalApiWithRetry()
        {
            Console.WriteLine("--- Сценарій 1: Зовнішній API + Retry ---");

            var retryPolicy = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetry(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)), // 2,4,8
                    onRetry: (exception, timeSpan, retryNumber, context) =>
                    {
                        Console.WriteLine(
                            $"[{DateTime.Now:HH:mm:ss}] Retry #{retryNumber} через {timeSpan.TotalSeconds}s. Причина: {exception.Message}");
                    });

            try
            {
                string result = retryPolicy.Execute(
                    () => CallExternalApi("https://api.example.com/data"));

                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] FINAL RESULT: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Операція провалилася після всіх повторів: {ex.Message}");
            }

            Console.WriteLine("--- Кінець сценарію 1 ---");
        }

        private static string CallExternalApi(string url)
        {
            _apiCallAttempts++;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Attempt #{_apiCallAttempts}: виклик API {url}...");

            if (_apiCallAttempts <= 2)
            {
                throw new HttpRequestException("Тимчасова помилка HTTP (імітація)");
            }

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] API {url} успішно відповів.");
            return "Data from API";
        }

     
        private static void Scenario2_DatabaseWithRetryAndCircuitBreaker()
        {
            Console.WriteLine("--- Сценарій 2: База даних + Retry + Circuit Breaker ---");

            var retryPolicy = Policy
                .Handle<Exception>() 
                .WaitAndRetry(
                    retryCount: 2,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(1),
                    onRetry: (exception, timeSpan, retryNumber, context) =>
                    {
                        Console.WriteLine(
                            $"[{DateTime.Now:HH:mm:ss}] [DB] Retry #{retryNumber} через {timeSpan.TotalSeconds}s. Причина: {exception.Message}");
                    });

            var circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreaker(
                    exceptionsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromSeconds(5),
                    onBreak: (exception, timeSpan) =>
                    {
                        Console.WriteLine(
                            $"[{DateTime.Now:HH:mm:ss}] [DB] Circuit OPEN на {timeSpan.TotalSeconds}s. Остання помилка: {exception.Message}");
                    },
                    onReset: () =>
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [DB] Circuit CLOSED (відновлено).");
                    },
                    onHalfOpen: () =>
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [DB] Circuit HALF-OPEN (тестовий запит).");
                    });

            var dbPolicy = Policy.Wrap(retryPolicy, circuitBreakerPolicy);

            for (int i = 1; i <= 8; i++)
            {
                try
                {
                    Console.WriteLine($"\n[{DateTime.Now:HH:mm:ss}] === DB запит #{i} ===");
                    string data = dbPolicy.Execute(() => FakeDatabaseQuery());
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Результат запиту: {data}");
                }
                catch (BrokenCircuitException)
                {
                    Console.WriteLine(
                        $"[{DateTime.Now:HH:mm:ss}] Виклик пропущено – ланцюг у стані Open (BrokenCircuitException).");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Помилка під час запиту до БД: {ex.Message}");
                }

                Thread.Sleep(1000); 
            }

            Console.WriteLine("--- Кінець сценарію 2 ---");
        }

        private static string FakeDatabaseQuery()
        {
            _dbCallAttempts++;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [DB] Спроба #{_dbCallAttempts}: виконання запиту...");

            if (_dbCallAttempts <= 5)
            {
                throw new Exception("Тимчасова помилка підключення до БД (імітація).");
            }

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [DB] Запит виконано успішно.");
            return "Row from DB";
        }

       
        private static void Scenario3_LongRunningOperationWithTimeoutAndFallback()
        {
            Console.WriteLine("--- Сценарій 3: Довга операція + Timeout + Fallback ---");

            
            var timeoutPolicy = Policy.Timeout<string>(
                timeout: TimeSpan.FromSeconds(2),
                timeoutStrategy: TimeoutStrategy.Pessimistic,
                onTimeout: (context, timeSpan, task, exception) =>
                {
                    Console.WriteLine(
                        $"[{DateTime.Now:HH:mm:ss}] [REPORT] Timeout після {timeSpan.TotalSeconds}s. Операція буде перервана.");
                });

            
            var fallbackPolicy = Policy<string>
                .Handle<TimeoutRejectedException>()
                .Fallback(
                    fallbackValue: "⚠ Неможливо згенерувати повний звіт – повернуто спрощену версію.",
                    onFallback: exception =>
                    {
                        Console.WriteLine(
                            $"[{DateTime.Now:HH:mm:ss}] [REPORT] Спрацював fallback: {exception.GetType().Name}");
                    });

            var reportPolicy = fallbackPolicy.Wrap(timeoutPolicy);

            try
            {
                
                for (int i = 1; i <= 3; i++)
                {
                    Console.WriteLine($"\n[{DateTime.Now:HH:mm:ss}] === Генерація звіту, спроба #{i} ===");
                    string report = reportPolicy.Execute(() => GenerateReport());
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Результат: {report}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Несподівана помилка: {ex.Message}");
            }

            Console.WriteLine("--- Кінець сценарію 3 ---");
        }

        
        private static string GenerateReport()
        {
            _reportAttempts++;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [REPORT] Спроба #{_reportAttempts}: генерація звіту...");

            int sleepSeconds = _reportAttempts <= 2 ? 4 : 1;
            Thread.Sleep(TimeSpan.FromSeconds(sleepSeconds));

            return $" Повний звіт згенеровано за {sleepSeconds} сек.";
        }
    }
}
