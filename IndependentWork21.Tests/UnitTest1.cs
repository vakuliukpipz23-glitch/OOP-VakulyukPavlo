using System;
using System.IO;
using Xunit;
using IndependentWork20;

public class IntegrationTests
{
    // POSITIVE 1
    [Fact]
    public void Strategy_UpperCase_Should_Process_Text()
    {
        var context = new DataContext(new UpperCaseStrategy());

        var result = context.ExecuteProcessing("Hello World");

        Assert.Equal("HELLO WORLD", result);
    }

    // POSITIVE 2
    [Fact]
    public void Strategy_Should_Switch_At_Runtime()
    {
        var context = new DataContext(new LowerCaseStrategy());

        var r1 = context.ExecuteProcessing("HeLLo");
        context.SetStrategy(new ReverseStringStrategy());
        var r2 = context.ExecuteProcessing("abc");

        Assert.Equal("hello", r1);
        Assert.Equal("cba", r2);
    }

    // POSITIVE 3
    [Fact]
    public void Observer_Should_Notify_Concrete_Subscribers()
    {
        var publisher = new DataPublisher();
        var consoleObserver = new ConsoleOutputObserver();
        var lengthObserver = new LengthLoggerObserver();

        publisher.DataProcessed += consoleObserver.OnDataProcessed;
        publisher.DataProcessed += lengthObserver.OnDataProcessed;

        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        try
        {
            publisher.PublishDataProcessed("TEST");
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = sw.ToString();
        Assert.Contains("ConsoleOutput: TEST", output);
        Assert.Contains("Length: 4", output);
    }

    // NEGATIVE/BOUNDARY 1
    [Fact]
    public void Strategy_Should_Throw_On_Null_Input()
    {
        var context = new DataContext(new UpperCaseStrategy());

        var ex = Record.Exception(() => context.ExecuteProcessing(null!));

        Assert.NotNull(ex);
        Assert.IsType<NullReferenceException>(ex);
    }

    // NEGATIVE/BOUNDARY 2
    [Fact]
    public void Publisher_Should_Not_Throw_Without_Subscribers()
    {
        var publisher = new DataPublisher();

        var ex = Record.Exception(() => publisher.PublishDataProcessed("data"));

        Assert.Null(ex);
    }
}