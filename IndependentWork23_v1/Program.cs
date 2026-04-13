using System;
using System.Collections.Generic;

namespace IndependentWork23
{
    // =========================
    // ADAPTER
    // =========================

    public interface ILogger
    {
        void Log(string message);
    }

    // Adaptee (стара бібліотека)
    public class OldLoggerLibrary
    {
        public void WriteLog(string msg)
        {
            Console.WriteLine($"[OLD LOGGER] {msg}");
        }
    }

    // Adapter
    public class OldLoggerAdapter : ILogger
    {
        private readonly OldLoggerLibrary _oldLogger;

        public OldLoggerAdapter(OldLoggerLibrary oldLogger)
        {
            _oldLogger = oldLogger;
        }

        public void Log(string message)
        {
            _oldLogger.WriteLog(message);
        }
    }

    // =========================
    // FACADE
    // =========================

    public class EmailService
    {
        public void Send(string message)
        {
            Console.WriteLine($"Email sent: {message}");
        }
    }

    public class SmsService
    {
        public void Send(string message)
        {
            Console.WriteLine($"SMS sent: {message}");
        }
    }

    public class NotificationFacade
    {
        private readonly EmailService _email = new EmailService();
        private readonly SmsService _sms = new SmsService();

        public void SendAll(string message)
        {
            _email.Send(message);
            _sms.Send(message);
        }
    }

    // =========================
    // PROXY
    // =========================

    public interface IDataLoader
    {
        string LoadData(string key);
    }

    // RealSubject
    public class RealDataLoader : IDataLoader
    {
        public string LoadData(string key)
        {
            Console.WriteLine($"[DB] Loading data for {key}");
            return $"DATA_FOR_{key}";
        }
    }

    // Proxy (Caching)
    public class CachingDataLoaderProxy : IDataLoader
    {
        private readonly RealDataLoader _real = new RealDataLoader();
        private readonly Dictionary<string, string> _cache = new Dictionary<string, string>();

        public string LoadData(string key)
        {
            if (_cache.ContainsKey(key))
            {
                Console.WriteLine($"[CACHE] Returning cached data for {key}");
                return _cache[key];
            }

            var data = _real.LoadData(key);
            _cache[key] = data;
            return data;
        }
    }

    // =========================
    // MAIN
    // =========================

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== ADAPTER ===");
            ILogger logger = new OldLoggerAdapter(new OldLoggerLibrary());
            logger.Log("Hello from adapter");

            Console.WriteLine("\n=== FACADE ===");
            var facade = new NotificationFacade();
            facade.SendAll("System update available");

            Console.WriteLine("\n=== PROXY ===");
            IDataLoader loader = new CachingDataLoaderProxy();

            Console.WriteLine(loader.LoadData("user1"));
            Console.WriteLine(loader.LoadData("user1")); // кеш
            Console.WriteLine(loader.LoadData("user2"));
        }
    }
}