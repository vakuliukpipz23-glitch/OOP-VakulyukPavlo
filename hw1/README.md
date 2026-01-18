# Анти-патерн God Object та принцип SRP

## 1. Характеристики анти-патерну **God Object**

**God Object** — це анти-патерн проєктування, при якому один клас:

* містить занадто багато полів і методів;
* знає майже про всі інші класи системи;
* виконує багато різних, не повʼязаних між собою відповідальностей;
* важкий для тестування, підтримки та розширення.

Основна проблема God Object полягає у порушенні принципу **SRP (Single Responsibility Principle)** — клас має лише одну причину для змін.

---

## 2. Приклад класу, який порушує SRP

Нижче наведено приклад простого класу на C#, який порушує SRP. Клас відповідає одразу за бізнес-логіку, роботу з базою даних та логування.

```csharp
public class UserManager
{
    public void CreateUser(string name)
    {
        Console.WriteLine("Лог: створення користувача");
        
        // Бізнес-логіка
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Імʼя не може бути порожнім");
        }

        // Робота з базою даних (умовно)
        Console.WriteLine($"INSERT INTO Users (Name) VALUES ('{name}')");
    }
}
```

### Чому цей клас порушує SRP?

Клас `UserManager` має декілька причин для змін:

* зміниться логіка логування;
* зміниться спосіб збереження даних;
* зміняться правила валідації користувача.

Це означає, що клас має **декілька відповідальностей**, що є прямим порушенням SRP.

---

## 3. Рефакторинг класу для дотримання SRP

Для дотримання SRP розділимо відповідальності на окремі класи:

* `UserValidator` — відповідає за валідацію;
* `UserRepository` — відповідає за збереження даних;
* `Logger` — відповідає за логування;
* `UserService` — координує роботу інших класів.

### Приклад після рефакторингу

```csharp
public class UserValidator
{
    public void Validate(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Імʼя не може бути порожнім");
        }
    }
}

public class UserRepository
{
    public void Save(string name)
    {
        Console.WriteLine($"INSERT INTO Users (Name) VALUES ('{name}')");
    }
}

public class Logger
{
    public void Log(string message)
    {
        Console.WriteLine($"Лог: {message}");
    }
}

public class UserService
{
    private readonly UserValidator _validator = new UserValidator();
    private readonly UserRepository _repository = new UserRepository();
    private readonly Logger _logger = new Logger();

    public void CreateUser(string name)
    {
        _logger.Log("створення користувача");
        _validator.Validate(name);
        _repository.Save(name);
    }
}
```

### Переваги рефакторингу

* кожен клас має **одну відповідальність**;
* код легше тестувати;
* простіше вносити зміни;
* зменшується ризик появи God Object.

---

## 4. Висновок

Анти-патерн **God Object** ускладнює підтримку системи та порушує принципи SOLID. Дотримання **SRP** дозволяє створювати гнучкий, зрозумілий та масштабований код.

