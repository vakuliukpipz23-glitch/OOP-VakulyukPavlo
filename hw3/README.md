# Принципи ISP та DIP (SOLID)

## Вступ

У цій роботі я розглядаю два принципи з набору SOLID — **ISP (Interface Segregation Principle)** та **DIP (Dependency Inversion Principle)**. Ці принципи допомагають будувати гнучкий, зрозумілий та зручний для тестування код.

## Interface Segregation Principle (ISP)

**Суть принципу:** не слід змушувати клас реалізовувати інтерфейси або методи, які він фактично не використовує.

### Приклад порушення ISP

```csharp
public interface IWorker
{
    void Work();
    void Eat();
}

public class Robot : IWorker
{
    public void Work()
    {
        Console.WriteLine("Robot is working");
    }

    public void Eat()
    {
        throw new NotImplementedException();
    }
}
```

У цьому прикладі інтерфейс `IWorker` містить метод `Eat`, який не має сенсу для класу `Robot`. В результаті клас змушений реалізовувати зайву для себе поведінку, що є порушенням ISP.

### Виправлення проблеми

```csharp
public interface IWorkable
{
    void Work();
}

public interface IEatable
{
    void Eat();
}

public class Human : IWorkable, IEatable
{
    public void Work() => Console.WriteLine("Human is working");
    public void Eat() => Console.WriteLine("Human is eating");
}

public class Robot : IWorkable
{
    public void Work() => Console.WriteLine("Robot is working");
}
```

Після розділення інтерфейсу кожен клас реалізує тільки ті методи, які йому дійсно потрібні. Це робить код чистішим і зрозумілішим.

## Dependency Inversion Principle (DIP)

**Суть принципу:** класи верхнього рівня не повинні залежати від конкретних реалізацій класів нижнього рівня — усі залежності мають будуватися через абстракції.

### Приклад без застосування DIP

```csharp
public class FileLogger
{
    public void Log(string message)
    {
        Console.WriteLine(message);
    }
}

public class OrderService
{
    private readonly FileLogger _logger = new FileLogger();
}
```

Тут клас `OrderService` напряму залежить від `FileLogger`, тому змінити спосіб логування або протестувати клас стає складно.

### Застосування DIP через Dependency Injection

```csharp
public interface ILogger
{
    void Log(string message);
}

public class FileLogger : ILogger
{
    public void Log(string message)
    {
        Console.WriteLine(message);
    }
}

public class OrderService
{
    private readonly ILogger _logger;

    public OrderService(ILogger logger)
    {
        _logger = logger;
    }
}
```

У цьому випадку `OrderService` працює з абстракцією `ILogger`, а конкретна реалізація передається ззовні.

### Переваги використання DIP і DI

* зменшується жорстка звʼязність між класами;
* реалізації легко змінювати без правок основної логіки;
* значно спрощується модульне тестування;
* код стає гнучкішим і легшим у підтримці.

## Звʼязок ISP, DI та тестування

Використання **вузьких інтерфейсів** напряму покращує Dependency Injection та тестування. Коли інтерфейс містить лише необхідні методи:

* його простіше замокати або підмінити в тестах;
* класи залежать тільки від потрібної поведінки;
* зменшується кількість побічних змін при розширенні системи.

Таким чином, ISP і DIP добре доповнюють один одного.

## Висновок

Застосування принципів ISP та DIP дозволяє створювати чисту архітектуру, у якій компоненти слабо звʼязані між собою, легко тестуються та розширюються. На практиці ці принципи значно спрощують підтримку коду та його подальший розвиток.
