using lab28v1.Models;
using lab28v1.Repository;

class Program
{
    static async Task Main(string[] args)
    {
        var repo = new BookRepository();

        // Створення об'єктів
        var book1 = new Book(1, "Clean Code", 2008,
            new Author(1, "Robert Martin"));

        var book2 = new Book(2, "The Pragmatic Programmer", 1999,
            new Author(2, "Andrew Hunt"));

        // Додавання
        repo.Add(book1);
        repo.Add(book2);

        // Збереження у файл
        string file = "books.json";
        await repo.SaveToFileAsync(file);

        Console.WriteLine("Дані збережено у JSON файл.\n");

        // Очистимо і завантажимо з файлу
        var repo2 = new BookRepository();
        await repo2.LoadFromFileAsync(file);

        // Вивід результату
        Console.WriteLine("Завантажені книги:");

        foreach (var book in repo2.GetAll())
        {
            Console.WriteLine($"{book.Id}: {book.Title} ({book.Year}) - {book.Author.FullName}");
        }

        // GetById приклад
        var single = repo2.GetById(1);
        Console.WriteLine($"\nЗнайдено по ID: {single.Title}");
    }
}
