using System;

namespace IndependentWork16
{
    // Поганий клас (порушує SRP)
    class OrderProcessor
    {
        public void ProcessOrder(string order)
        {
            Validate(order);
            SaveToDatabase(order);
            SendEmail(order);
            Console.WriteLine("Замовлення оброблено (поганий дизайн).");
        }

        private void Validate(string order)
        {
            Console.WriteLine("Валідація замовлення...");
        }

        private void SaveToDatabase(string order)
        {
            Console.WriteLine("Збереження замовлення в базу даних...");
        }

        private void SendEmail(string order)
        {
            Console.WriteLine("Відправка email-сповіщення...");
        }
    }

    // Інтерфейси (SRP + DIP)
    interface IOrderValidator
    {
        bool Validate(string order);
    }

    interface IOrderRepository
    {
        void Save(string order);
    }

    interface IEmailService
    {
        void Send(string order);
    }

    // Реалізації (заглушки)
    class OrderValidator : IOrderValidator
    {
        public bool Validate(string order)
        {
            Console.WriteLine("Валідація замовлення...");
            return true;
        }
    }

    class OrderRepository : IOrderRepository
    {
        public void Save(string order)
        {
            Console.WriteLine("Збереження замовлення в БД...");
        }
    }

    class EmailService : IEmailService
    {
        public void Send(string order)
        {
            Console.WriteLine("Відправка email-сповіщення...");
        }
    }

    // OrderService — координатор
    class OrderService
    {
        private readonly IOrderValidator _validator;
        private readonly IOrderRepository _repository;
        private readonly IEmailService _emailService;

        public OrderService(
            IOrderValidator validator,
            IOrderRepository repository,
            IEmailService emailService)
        {
            _validator = validator;
            _repository = repository;
            _emailService = emailService;
        }

        public void ProcessOrder(string order)
        {
            if (_validator.Validate(order))
            {
                _repository.Save(order);
                _emailService.Send(order);
                Console.WriteLine("Замовлення успішно оброблено (SRP).");
            }
            else
            {
                Console.WriteLine("Помилка валідації замовлення.");
            }
        }
    }

    class Program
    {
        static void Main()
        {
            // Демонстрація поганого класу
            Console.WriteLine("=== Поганий дизайн ===");
            OrderProcessor badProcessor = new OrderProcessor();
            badProcessor.ProcessOrder("Order #1");

            Console.WriteLine("\n=== Рефакторинг (SRP + DIP) ===");

            // Демонстрація правильного дизайну
            IOrderValidator validator = new OrderValidator();
            IOrderRepository repository = new OrderRepository();
            IEmailService emailService = new EmailService();

            OrderService orderService =
                new OrderService(validator, repository, emailService);

            orderService.ProcessOrder("Order #2");

            Console.ReadLine();
        }
    }
}
