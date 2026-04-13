using System;

namespace IndependentWork20
{
    // ================= STRATEGY =================

    public interface IDataProcessorStrategy
    {
        string Process(string data);
    }

    public class UpperCaseStrategy : IDataProcessorStrategy
    {
        public string Process(string data)
        {
            return data.ToUpper();
        }
    }

    public class LowerCaseStrategy : IDataProcessorStrategy
    {
        public string Process(string data)
        {
            return data.ToLower();
        }
    }

    public class ReverseStringStrategy : IDataProcessorStrategy
    {
        public string Process(string data)
        {
            char[] arr = data.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }
    }

    public class DataContext
    {
        private IDataProcessorStrategy strategy;

        public DataContext(IDataProcessorStrategy strategy)
        {
            this.strategy = strategy;
        }

        public void SetStrategy(IDataProcessorStrategy strategy)
        {
            this.strategy = strategy;
        }

        public string ExecuteProcessing(string data)
        {
            return strategy.Process(data);
        }
    }

    // ================= OBSERVER =================

    public class DataPublisher
    {
        public event Action<string> DataProcessed;

        public void PublishDataProcessed(string data)
        {
            DataProcessed?.Invoke(data);
        }
    }

    public class ConsoleOutputObserver
    {
        public void OnDataProcessed(string data)
        {
            Console.WriteLine($"ConsoleOutput: {data}");
        }
    }

    public class LengthLoggerObserver
    {
        public void OnDataProcessed(string data)
        {
            Console.WriteLine($"Length: {data.Length}");
        }
    }

    // ================= MAIN =================

    class Program
    {
        static void Main(string[] args)
        {
            var publisher = new DataPublisher();

            var consoleObserver = new ConsoleOutputObserver();
            var lengthObserver = new LengthLoggerObserver();

            // Підписка
            publisher.DataProcessed += consoleObserver.OnDataProcessed;
            publisher.DataProcessed += lengthObserver.OnDataProcessed;

            var context = new DataContext(new UpperCaseStrategy());

            string input = "Hello World";

            // UpperCase
            var result1 = context.ExecuteProcessing(input);
            publisher.PublishDataProcessed(result1);

            Console.WriteLine();

            // LowerCase
            context.SetStrategy(new LowerCaseStrategy());
            var result2 = context.ExecuteProcessing(input);
            publisher.PublishDataProcessed(result2);

            Console.WriteLine();

            // Reverse
            context.SetStrategy(new ReverseStringStrategy());
            var result3 = context.ExecuteProcessing(input);
            publisher.PublishDataProcessed(result3);
        }
    }
}