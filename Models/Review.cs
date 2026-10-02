namespace Final_Project.Models
{
    public class Review
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public string CustomerId { get; set; } = string.Empty;
        public ApplicationUser? Customer { get; set; }

        public int Rating { get; set; } 
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}