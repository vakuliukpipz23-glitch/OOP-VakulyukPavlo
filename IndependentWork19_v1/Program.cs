using System;

namespace IndependentWork19
{
    // 1. Інтерфейс
    public interface INotificationSender
    {
        void Send(string message);
    }

    // 2. Реалізації

    public class EmailSender : INotificationSender
    {
        public void Send(string message)
        {
            Console.WriteLine($"[EMAIL] {message}");
        }
    }

    public class SmsSender : INotificationSender
    {
        public void Send(string message)
        {
            Console.WriteLine($"[SMS] {message}");
        }
    }

    public class PushSender : INotificationSender
    {
        public void Send(string message)
        {
            Console.WriteLine($"[PUSH] {message}");
        }
    }

    // 3. Абстрактна фабрика
    public abstract class NotificationSenderFactory
    {
        protected abstract INotificationSender CreateSender();

        public void SendMessage(string message)
        {
            var sender = CreateSender();
            sender.Send(message);
        }
    }

    // 4. Конкретні фабрики

    public class EmailSenderFactory : NotificationSenderFactory
    {
        protected override INotificationSender CreateSender()
        {
            return new EmailSender();
        }
    }

    public class SmsSenderFactory : NotificationSenderFactory
    {
        protected override INotificationSender CreateSender()
        {
            return new SmsSender();
        }
    }

    public class PushSenderFactory : NotificationSenderFactory
    {
        protected override INotificationSender CreateSender()
        {
            return new PushSender();
        }
    }

    // 5. Singleton
    public class NotificationService
    {
        private static NotificationService instance;
        private NotificationSenderFactory factory;

        private NotificationService() { }

        public static NotificationService GetInstance()
        {
            if (instance == null)
            {
                instance = new NotificationService();
            }
            return instance;
        }

        public void SetFactory(NotificationSenderFactory factory)
        {
            this.factory = factory;
        }

        public void Send(string message)
        {
            if (factory == null)
            {
                Console.WriteLine("Фабрика не встановлена!");
                return;
            }

            factory.SendMessage(message);
        }
    }

    // 6. Main
    class Program
    {
        static void Main(string[] args)
        {
            var service = NotificationService.GetInstance();

            // Email
            service.SetFactory(new EmailSenderFactory());
            service.Send("Привіт через Email");
            service.Send("Ще одне Email повідомлення");

            Console.WriteLine();

            // SMS
            service.SetFactory(new SmsSenderFactory());
            service.Send("Привіт через SMS");
            service.Send("Ще одне SMS повідомлення");

            Console.WriteLine();

            // Push
            service.SetFactory(new PushSenderFactory());
            service.Send("Привіт через Push");
            service.Send("Ще одне Push повідомлення");
        }
    }
}