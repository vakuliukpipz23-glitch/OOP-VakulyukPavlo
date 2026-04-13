using System.Text.Json;
using lab28v1.Models;

namespace lab28v1.Repository
{
    public class BookRepository
    {
        private List<Book> _books = new();

        public void Add(Book book)
        {
            _books.Add(book);
        }

        public List<Book> GetAll()
        {
            return _books;
        }

        public Book GetById(int id)
        {
            return _books.FirstOrDefault(b => b.Id == id);
        }

        public async Task SaveToFileAsync(string filename)
        {
            var json = JsonSerializer.Serialize(_books, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(filename, json);
        }

        public async Task LoadFromFileAsync(string filename)
        {
            if (!File.Exists(filename))
            {
                _books = new List<Book>();
                return;
            }

            var json = await File.ReadAllTextAsync(filename);

            _books = JsonSerializer.Deserialize<List<Book>>(json) ?? new List<Book>();
        }
    }
}
