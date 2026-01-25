using System;

namespace lab21
{
    // 1. Інтерфейс стратегії
    public interface IShippingStrategy
    {
        decimal CalculateCost(decimal distance, decimal weight);
    }

    // 2. Реалізації стратегій
    public class StandardShippingStrategy : IShippingStrategy
    {
        public decimal CalculateCost(decimal distance, decimal weight)
        {
            return distance * 1.5m + weight * 0.5m;
        }
    }

    public class ExpressShippingStrategy : IShippingStrategy
    {
        public decimal CalculateCost(decimal distance, decimal weight)
        {
            return distance * 2.5m + weight * 1.0m + 50m;
        }
    }

    public class InternationalShippingStrategy : IShippingStrategy
    {
        public decimal CalculateCost(decimal distance, decimal weight)
        {
            decimal baseCost = distance * 5.0m + weight * 2.0m;
            return baseCost + baseCost * 0.15m; // 15% податок
        }
    }

    // 3. Нова стратегія (демонстрація OCP)
    public class NightShippingStrategy : IShippingStrategy
    {
        public decimal CalculateCost(decimal distance, decimal weight)
        {
            decimal standardCost = distance * 1.5m + weight * 0.5m;
            return standardCost + 30m; // нічна націнка
        }
    }

    // 4. Factory Method
    public static class ShippingStrategyFactory
    {
        public static IShippingStrategy CreateStrategy(string deliveryType)
        {
            return deliveryType.ToLower() switch
            {
                "standard" => new StandardShippingStrategy(),
                "express" => new ExpressShippingStrategy(),
                "international" => new InternationalShippingStrategy(),
                "night" => new NightShippingStrategy(),
                _ => throw new ArgumentException("Невідомий тип доставки")
            };
        }
    }

    // 5. Сервіс доставки
    public class DeliveryService
    {
        public decimal CalculateDeliveryCost(
            decimal distance,
            decimal weight,
            IShippingStrategy strategy)
        {
            return strategy.CalculateCost(distance, weight);
        }
    }

    // 6. Точка входу
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Оберіть тип доставки:");
            Console.WriteLine("Standard | Express | International | Night");
            string type = Console.ReadLine();

            Console.Write("Введіть відстань (км): ");
            decimal distance = decimal.Parse(Console.ReadLine());

            Console.Write("Введіть вагу (кг): ");
            decimal weight = decimal.Parse(Console.ReadLine());

            try
            {
                IShippingStrategy strategy =
                    ShippingStrategyFactory.CreateStrategy(type);

                DeliveryService service = new DeliveryService();
                decimal cost = service.CalculateDeliveryCost(distance, weight, strategy);

                Console.WriteLine($"Вартість доставки: {cost} грн");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }

            Console.ReadLine();
        }
    }
}
