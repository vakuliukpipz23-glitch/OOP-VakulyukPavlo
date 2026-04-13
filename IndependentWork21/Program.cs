using System;

namespace IndependentWork21
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
            if (data is null) throw new ArgumentNullException(nameof(data));
            return data.ToUpperInvariant();
        }
    }

    public class LowerCaseStrategy : IDataProcessorStrategy
    {
        public string Process(string data)
        {
            if (data is null) throw new ArgumentNullException(nameof(data));
            return data.ToLowerInvariant();
        }
    }

    public class ReverseStringStrategy : IDataProcessorStrategy
    {
        public string Process(string data)
        {
            if (data is null) throw new ArgumentNullException(nameof(data));
            char[] arr = data.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }
    }

    public class DataContext
    {
        private IDataProcessorStrategy _strategy;

        public DataContext(IDataProcessorStrategy strategy)
        {
            _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
        }

        public void SetStrategy(IDataProcessorStrategy strategy)
        {
            _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
        }

        public string ExecuteProcessing(string data)
        {
            return _strategy.Process(data);
        }
    }

    // ================= FACTORY =================

    public static class DataProcessorFactory
    {
        public static IDataProcessorStrategy Create(string strategyName)
        {
            if (string.IsNullOrWhiteSpace(strategyName))
                throw new ArgumentException("Strategy name cannot be empty.", nameof(strategyName));

            return strategyName.Trim().ToLowerInvariant() switch
            {
                "upper" => new UpperCaseStrategy(),
                "lower" => new LowerCaseStrategy(),
                "reverse" => new ReverseStringStrategy(),
                _ => throw new ArgumentException($"Unknown strategy: {strategyName}", nameof(strategyName))
            };
        }
    }

    // ================= SINGLETON =================

    public sealed class ProcessingState
    {
        private static readonly Lazy<ProcessingState> _instance =
            new Lazy<ProcessingState>(() => new ProcessingState());

        public static ProcessingState Instance => _instance.Value;

        private ProcessingState() { }

        public int ProcessedCount { get; private set; }
        public string LastResult { get; private set; } = string.Empty;

        public void Register(string result)
        {
            if (result is null) throw new ArgumentNullException(nameof(result));
            ProcessedCount++;
            LastResult = result;
        }

        public void ResetForTests()
        {
            ProcessedCount = 0;
            LastResult = string.Empty;
        }
    }

    // ================= OBSERVER =================

    public class DataPublisher
    {
        public event Action<string>? DataProcessed;

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
        public int InvocationCount { get; private set; }
        public int LastLength { get; private set; }

        public void OnDataProcessed(string data)
        {
            InvocationCount++;
            LastLength = data?.Length ?? 0;
            Console.WriteLine($"Length: {LastLength}");
        }
    }

    // ================= INTEGRATION PIPELINE =================

    public class DataProcessingPipeline
    {
        private readonly DataContext _context;
        private readonly DataPublisher _publisher;

        public DataProcessingPipeline(DataPublisher publisher)
        {
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
            _context = new DataContext(new UpperCaseStrategy());
        }

        public string ProcessWithStrategy(string strategyName, string input)
        {
            var strategy = DataProcessorFactory.Create(strategyName);
            _context.SetStrategy(strategy);

            var result = _context.ExecuteProcessing(input);

            ProcessingState.Instance.Register(result);
            _publisher.PublishDataProcessed(result);

            return result;
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

            publisher.DataProcessed += consoleObserver.OnDataProcessed;
            publisher.DataProcessed += lengthObserver.OnDataProcessed;

            var pipeline = new DataProcessingPipeline(publisher);
            string input = "Hello World";

            pipeline.ProcessWithStrategy("upper", input);
            Console.WriteLine();

            pipeline.ProcessWithStrategy("lower", input);
            Console.WriteLine();

            pipeline.ProcessWithStrategy("reverse", input);
            Console.WriteLine();

            Console.WriteLine($"ProcessedCount: {ProcessingState.Instance.ProcessedCount}");
            Console.WriteLine($"LastResult: {ProcessingState.Instance.LastResult}");
        }
    }
}