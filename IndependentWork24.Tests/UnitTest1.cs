using System;
using Xunit;
using IndependentWork24;

public class IntegrationTests
{
    [Fact]
    public void Composite_Should_Sum_Nested_Sizes()
    {
        var root = new Folder("root");
        root.Add(new FileItem("a", 100));
        var sub = new Folder("sub");
        sub.Add(new FileItem("b", 200));
        root.Add(sub);

        var total = root.GetSizeBytes();

        Assert.Equal(300, total);
    }

    [Fact]
    public void Decorators_Should_Apply_In_Order()
    {
        IStorageComponent file = new FileItem("a", 1000);
        IStorageComponent decorated = new EncryptionDecorator(
            new CompressionDecorator(file, 50), 100);

        var result = decorated.GetSizeBytes();

        Assert.Equal(600, result); // (1000 * 50%) + 100
    }

    [Fact]
    public void Proxy_Should_Cache_Result()
    {
        IStorageComponent file = new FileItem("a", 1234);
        var proxy = new CachedSizeProxy(file);

        var r1 = proxy.GetSizeBytes();
        var r2 = proxy.GetSizeBytes();

        Assert.Equal(r1, r2);
        Assert.Equal(1, proxy.CacheMisses);
        Assert.True(proxy.CacheHits >= 1);
    }

    [Fact]
    public void Empty_Folder_Should_Return_Zero()
    {
        var empty = new Folder("empty");

        var result = empty.GetSizeBytes();

        Assert.Equal(0, result);
    }

    [Fact]
    public void Compression_Should_Throw_For_Invalid_Ratio()
    {
        IStorageComponent file = new FileItem("a", 10);

        Assert.Throws<ArgumentOutOfRangeException>(() => new CompressionDecorator(file, 0));
    }
}