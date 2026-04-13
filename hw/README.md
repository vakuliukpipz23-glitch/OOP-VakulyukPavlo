# JSON vs XML: практичне порівняння

## 1. Вступ
JSON та XML — це два популярні формати для зберігання та обміну даними. Обидва використовуються для серіалізації об’єктів, передачі даних між системами та збереження структурованої інформації. Проте вони мають різну структуру, синтаксис і сфери застосування.

## 2. Приклад класу (C#)
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; }
    public bool InStock { get; set; }
}

## 3. Серіалізація в JSON
{
  "Id": 1,
  "Name": "Laptop",
  "Price": 25000.0,
  "Category": "Electronics",
  "InStock": true
}

## 4. Серіалізація в XML
<Product>
  <Id>1</Id>
  <Name>Laptop</Name>
  <Price>25000</Price>
  <Category>Electronics</Category>
  <InStock>true</InStock>
</Product>

## 5. Порівняння JSON і XML
| Критерій | JSON | XML |
|----------|------|-----|
| Читабельність | Вища | Нижча |
| Об’єм даних | Менший | Більший |
| Швидкість | Швидший | Повільніший |
| Структура | Простіша | Гнучка |
| Використання | Web/API | Enterprise |

## 6. Сценарії використання
JSON:
- REST API
- мобільні застосунки
- фронтенд

XML:
- банківські системи
- SOAP сервіси
- enterprise конфігурації

## 7. Висновок
JSON простіший і популярніший у вебі, XML більш строгий і використовується в корпоративних системах.
