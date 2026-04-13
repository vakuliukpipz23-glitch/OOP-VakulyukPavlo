namespace lab28v1.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string FullName { get; set; }

        public Author() { }

        public Author(int id, string fullName)
        {
            Id = id;
            FullName = fullName;
        }
    }
}