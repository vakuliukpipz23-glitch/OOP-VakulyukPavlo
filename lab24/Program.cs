using System;
using System.Collections.Generic;

#region Strategy

public interface INumericOperationStrategy
{
    double Execute(double value);
    string OperationName { get; }
}

public class SquareOperationStrategy : INumericOperationStrategy
{
    public string OperationName => "Square";

    public double Execute(double value)
    {
        return value * value;
    }
}

public class CubeOperationStrategy : INumericOperationStrategy
{
    public string OperationName => "Cube";

    public double Execute(double value)
    {
        return value * value * value;
    }
}

public class SquareRootOperationStrategy : INumericOperationStrategy
{
    public string OperationName => "Square Root";

    public double Execute(double value)
    {
        return Math.Sqrt(value);
    }
}

public class NumericProcessor
{
    private INumericOperationStrategy _strategy;

    public NumericProcessor(INumericOperationStrategy strategy)
    {
        _strategy = strategy;
    }

    public void SetStrategy(INumericOperationStrategy strategy)
    {
        _strategy = strategy;
    }

    public double Process(double input)
    {
        return _strategy.Execute(input);
    }

    public string CurrentOperation => _strategy.OperationName;
}

#endregion

#region Observer

public class ResultPublisher
{
    public event Action<double, string> ResultCalculated;

    public void PublishResult(double result, string operationName)
    {
        ResultCalculated?.Invoke(result, operationName);
    }
}

public class ConsoleLoggerObserver
{
    public void Subscribe(ResultPublisher publisher)
    {
        publisher.ResultCalculated += OnResultCalculated;
    }

    private void OnResultCalculated(double result, string operationName)
    {
        Console.WriteLine($"[Console] Operation: {operationName}, Result: {result}");
    }
}

public class HistoryLoggerObserver
{
    public List<string> History { get; } = new();

    public void Subscribe(ResultPublisher publisher)
    {
        publisher.ResultCalculated += OnResultCalculated;
    }

    private void OnResultCalculated(double result, string operationName)
    {
        History.Add($"{operationName}: {result}");
    }
}

public class ThresholdNotifierObserver
{
    private readonly double _threshold;

    public ThresholdNotifierObserver(double threshold)
    {
        _threshold = threshold;
    }

    public void Subscribe(ResultPublisher publisher)
    {
        publisher.ResultCalculated += OnResultCalculated;
    }

    private void OnResultCalculated(double result, string operationName)
    {
        if (result > _threshold)
        {
            Console.WriteLine($"⚠ Threshold exceeded! {operationName} result = {result}");
        }
    }
}

#endregion

class Program
{
    static void Main()
    {
        var publisher = new ResultPublisher();
        var processor = new NumericProcessor(new SquareOperationStrategy());

        var consoleObserver = new ConsoleLoggerObserver();
        var historyObserver = new HistoryLoggerObserver();
        var thresholdObserver = new ThresholdNotifierObserver(50);

        consoleObserver.Subscribe(publisher);
        historyObserver.Subscribe(publisher);
        thresholdObserver.Subscribe(publisher);

        double[] inputs = { 4, 9, 16 };

        Console.WriteLine("=== Square ===");
        foreach (var value in inputs)
        {
            var result = processor.Process(value);
            publisher.PublishResult(result, processor.CurrentOperation);
        }

        processor.SetStrategy(new CubeOperationStrategy());

        Console.WriteLine("\n=== Cube ===");
        foreach (var value in inputs)
        {
            var result = processor.Process(value);
            publisher.PublishResult(result, processor.CurrentOperation);
        }

        processor.SetStrategy(new SquareRootOperationStrategy());

        Console.WriteLine("\n=== Square Root ===");
        foreach (var value in inputs)
        {
            var result = processor.Process(value);
            publisher.PublishResult(result, processor.CurrentOperation);
        }

        Console.WriteLine("\n=== History ===");
        foreach (var record in historyObserver.History)
        {
            Console.WriteLine(record);
        }
    }
}
