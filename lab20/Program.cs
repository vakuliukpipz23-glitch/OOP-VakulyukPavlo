using System;
using System.Collections.Generic;

namespace lab20
{
    // ===== MODEL =====
    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Order(int id, string customerName, decimal totalAmount)
        {
            Id = id;
            CustomerName = customerName;
            TotalAmount = totalAmount;
            Status = OrderStatus.New;
        }
    }

    public enum OrderStatus
    {
        New,
        PendingValidation,
        Processed,
        Shipped,
        Delivered,
        Cancelled
    }

    // ===== INTERFACES =====
    public interface IOrderValidator
    {
        bool IsValid(Order order);
    }

    public interface IOrderRepository
    {
        void Save(Order order);
        Order GetById(int id);
    }

    public interface IEmailService
    {
        void SendOrderConfirmation(Order order);
    }

    // ===== IMPLEMENTATIONS =====
    public class OrderValidator : IOrderValidator
    {
        public bool IsValid(Order order)
        {
            return order.TotalAmount > 0;
        }
    }

    public class InMemoryOrderRepository : IOrderRepository
    {
        private readonly Dictionary<int, Order> _orders = new();

        public void Save(Order order)
        {
            _orders[order.Id] = order;
            Console.WriteLine($"[DB] Order {order.Id} saved.");
        }

        public Order GetById(int id)
        {
            return _orders.ContainsKey(id) ? _orders[id] : null;
        }
    }

    public class ConsoleEmailService : IEmailService
    {
        public void SendOrderConfirmation(Order order)
        {
            Console.WriteLine($"[EMAIL] Confirmation sent to {order.CustomerName}");
        }
    }

    // ===== SERVICE =====
    public class OrderService
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

        public void ProcessOrder(Order order)
        {
            Console.WriteLine($"\nProcessing order {order.Id}...");

            if (!_validator.IsValid(order))
            {
                Console.WriteLine("Order is invalid!");
                order.Status = OrderStatus.Cancelled;
                return;
            }

            order.Status = OrderStatus.Processed;
            _repository.Save(order);
            _emailService.SendOrderConfirmation(order);

            Console.WriteLine($"Order {order.Id} processed successfully.");
        }
    }

    // ===== MAIN =====
    class Program
    {
        static void Main()
        {
            IOrderValidator validator = new OrderValidator();
            IOrderRepository repository = new InMemoryOrderRepository();
            IEmailService emailService = new ConsoleEmailService();

            var orderService = new OrderService(validator, repository, emailService);

            var validOrder = new Order(1, "Ivan Petrenko", 1200m);
            orderService.ProcessOrder(validOrder);

            var invalidOrder = new Order(2, "Oleh Shevchenko", -300m);
            orderService.ProcessOrder(invalidOrder);

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
