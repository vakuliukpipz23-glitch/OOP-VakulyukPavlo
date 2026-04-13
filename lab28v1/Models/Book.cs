namespace lab28v1.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }

        public Author Author { get; set; }

        public Book() { }

        public Book(int id, string title, int year, Author author)
        {
            Id = id;
            Title = title;
            Year = year;
            Author = author;
        }
    }
}