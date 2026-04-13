using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace IndependentWork24;

// ================= COMPOSITE =================

public interface IStorageComponent
{
    string Name { get; }
    long GetSizeBytes();
}

public sealed class FileItem : IStorageComponent
{
    public string Name { get; }
    private readonly long _sizeBytes;

    public FileItem(string name, long sizeBytes)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.");
        if (sizeBytes < 0) throw new ArgumentOutOfRangeException(nameof(sizeBytes));

        Name = name;
        _sizeBytes = sizeBytes;
    }

    public long GetSizeBytes() => _sizeBytes;
}

public sealed class Folder : IStorageComponent
{
    public string Name { get; }
    private readonly List<IStorageComponent> _children = new();

    public Folder(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.");
        Name = name;
    }

    public void Add(IStorageComponent component)
    {
        _children.Add(component ?? throw new ArgumentNullException(nameof(component)));
    }

    public long GetSizeBytes()
    {
        long total = 0;
        foreach (var child in _children) total += child.GetSizeBytes();
        return total;
    }
}

// ================= DECORATOR =================

public abstract class StorageDecorator : IStorageComponent
{
    protected readonly IStorageComponent Inner;
    public virtual string Name => Inner.Name;

    protected StorageDecorator(IStorageComponent inner)
    {
        Inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    public abstract long GetSizeBytes();
}

public sealed class CompressionDecorator : StorageDecorator
{
    private readonly int _percentAfterCompression; // 1..100

    public CompressionDecorator(IStorageComponent inner, int percentAfterCompression) : base(inner)
    {
        if (percentAfterCompression is < 1 or > 100)
            throw new ArgumentOutOfRangeException(nameof(percentAfterCompression), "Range: 1..100");
        _percentAfterCompression = percentAfterCompression;
    }

    public override long GetSizeBytes()
    {
        return Inner.GetSizeBytes() * _percentAfterCompression / 100;
    }
}

public sealed class EncryptionDecorator : StorageDecorator
{
    private readonly long _overheadBytes;

    public EncryptionDecorator(IStorageComponent inner, long overheadBytes) : base(inner)
    {
        if (overheadBytes < 0) throw new ArgumentOutOfRangeException(nameof(overheadBytes));
        _overheadBytes = overheadBytes;
    }

    public override long GetSizeBytes()
    {
        return Inner.GetSizeBytes() + _overheadBytes;
    }
}

// ================= PROXY =================

public sealed class CachedSizeProxy : IStorageComponent
{
    private readonly IStorageComponent _inner;
    private long? _cached;

    public string Name => _inner.Name;
    public int CacheHits { get; private set; }
    public int CacheMisses { get; private set; }

    public CachedSizeProxy(IStorageComponent inner)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    public long GetSizeBytes()
    {
        if (_cached.HasValue)
        {
            CacheHits++;
            return _cached.Value;
        }

        CacheMisses++;
        _cached = _inner.GetSizeBytes();
        return _cached.Value;
    }

    public void Invalidate() => _cached = null;
}

// ================= MAIN =================

internal class Program
{
    static void Main()
    {
        var root = new Folder("root");
        root.Add(new FileItem("a.bin", 1_000_000));
        root.Add(new FileItem("b.bin", 2_000_000));

        var docs = new Folder("docs");
        docs.Add(new FileItem("report.pdf", 500_000));
        docs.Add(new FileItem("notes.txt", 50_000));
        root.Add(docs);

        var baseComponent = (IStorageComponent)root;
        var decorated = new EncryptionDecorator(new CompressionDecorator(baseComponent, 70), 1024);
        var proxied = new CachedSizeProxy(decorated);

        var baseMs = Measure(() => baseComponent.GetSizeBytes(), 30_000);
        var decoratedMs = Measure(() => decorated.GetSizeBytes(), 30_000);
        var proxiedMs = Measure(() => proxied.GetSizeBytes(), 30_000);

        Console.WriteLine($"Base size: {baseComponent.GetSizeBytes()} bytes");
        Console.WriteLine($"Decorated size: {decorated.GetSizeBytes()} bytes");
        Console.WriteLine($"Proxy size: {proxied.GetSizeBytes()} bytes");
        Console.WriteLine($"Proxy hits/misses: {proxied.CacheHits}/{proxied.CacheMisses}");

        Console.WriteLine();
        Console.WriteLine($"Base elapsed: {baseMs} ms");
        Console.WriteLine($"Decorated elapsed: {decoratedMs} ms");
        Console.WriteLine($"Proxied elapsed: {proxiedMs} ms");
    }

    private static long Measure(Func<long> action, int iterations)
    {
        var sw = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++) _ = action();
        sw.Stop();
        return sw.ElapsedMilliseconds;
    }
}
