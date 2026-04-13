# Самостійна робота №21 

## 1. Перелік сценаріїв

### Позитивні
1. Factory + Strategy + Observer + Singleton (`upper`).
2. Вибір strategy у runtime (`upper -> lower -> reverse`).
3. Стабільність singleton-стану в межах сценарію.

### Негативні / граничні
4. Невідома стратегія фабрики (`unknown`) -> `ArgumentException`.
5. `null` вхідні дані -> `ArgumentNullException`.
6. Порожній рядок (boundary) -> коректна обробка і сповіщення observer.

## 2. Очікуваний / фактичний результат

| № | Сценарій | Очікуваний | Фактичний |
|---|---|---|---|
| 1 | upper + взаємодія всіх патернів | Результат `HELLO`, observers викликані, singleton оновлений | Відповідає |
| 2 | runtime strategy switch | `ABC`, `abc`, `CbA`; лічильник `3` | Відповідає |
| 3 | singleton stability | Один екземпляр, стан накопичується | Відповідає |
| 4 | unknown strategy | `ArgumentException` | Відповідає |
| 5 | null input | `ArgumentNullException` | Відповідає |
| 6 | empty input | `""`, довжина `0`, observer викликані | Відповідає |

## 3. Короткий висновок по ризиках

- Ризик глобального стану singleton між тестами.  
  Рішення: `ResetForTests()` у `[TestInitialize]`.
- Ризик помилок при додаванні нових strategy у Factory.  
  Рішення: централізоване оновлення мапінгу + тести на нові ключі.
- Ризик надлишкових підписок observer у довгоживучих сценаріях.  
  Рішення: контроль підписки/відписки.