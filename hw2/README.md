# Принцип підстановки Лісков (LSP)

## Вступ

У цій доповіді я розглядаю кілька прикладів порушення **LSP (Liskov Substitution Principle)**. Згідно з цим принципом, обʼєкти похідного класу повинні без проблем замінювати обʼєкти базового класу без зміни коректності роботи програми.

## Приклад 1: Птах і Пінгвін

```csharp
public class Bird
{
    public virtual void Fly()
    {
        Console.WriteLine("Bird is flying");
    }
}

public class Penguin : Bird
{
    public override void Fly()
    {
        throw new NotSupportedException();
    }
}
```

### Чому це порушує LSP

Клас `Penguin` формально є нащадком `Bird`, але він не може виконувати метод `Fly`. Якщо в коді очікується, що будь-який `Bird` може літати, підстановка `Penguin` призведе до помилки під час виконання.

### Проблеми

* неочікувані винятки;
* необхідність додаткових перевірок типів;
* зниження надійності коду.

### Як виправити

```csharp
public interface IFlyable
{
    void Fly();
}

public class Bird
{
}

public class Sparrow : Bird, IFlyable
{
    public void Fly() => Console.WriteLine("Sparrow is flying");
}

public class Penguin : Bird
{
}
```

## Приклад 2: Банківський рахунок і заморожений рахунок

```csharp
public class BankAccount
{
    public virtual void Withdraw(decimal amount)
    {
        Console.WriteLine("Money withdrawn");
    }
}

public class FrozenAccount : BankAccount
{
    public override void Withdraw(decimal amount)
    {
        throw new InvalidOperationException("Account is frozen");
    }
}
```

### Чому це порушує LSP

Код, який працює з `BankAccount`, очікує, що метод `Withdraw` завжди доступний. Підстановка `FrozenAccount` ламає цю гарантію.

### Проблеми

* бізнес-логіка перестає бути передбачуваною;
* зʼявляються приховані залежності від конкретних типів.

### Як виправити

```csharp
public interface IWithdrawable
{
    void Withdraw(decimal amount);
}

public class ActiveAccount : IWithdrawable
{
    public void Withdraw(decimal amount)
    {
        Console.WriteLine("Money withdrawn");
    }
}

public class FrozenAccount
{
}
```

## Приклад 3: Черга і обмежена черга

```csharp
public class Queue
{
    public virtual void Enqueue(int item)
    {
        Console.WriteLine("Item added");
    }
}

public class LimitedQueue : Queue
{
    public override void Enqueue(int item)
    {
        throw new InvalidOperationException("Queue is full");
    }
}
```

### Чому це порушує LSP

Базовий клас дозволяє додавання елементів без обмежень, але похідний клас змінює цю поведінку, що робить його несумісним з очікуваннями клієнтського коду.

### Як виправити

```csharp
public interface IQueue
{
    bool TryEnqueue(int item);
}

public class SimpleQueue : IQueue
{
    public bool TryEnqueue(int item) => true;
}

public class LimitedQueue : IQueue
{
    public bool TryEnqueue(int item)
    {
        return false;
    }
}
```

## Висновок

У всіх наведених прикладах порушення LSP виникає через те, що похідні класи звужують або змінюють поведінку базових класів. Для дотримання LSP важливо правильно проєктувати ієрархії, виділяти окремі інтерфейси та не змінювати очікувані контракти базових класів.
