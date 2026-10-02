namespace Final_Project.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        public string CustomerId { get; set; } = string.Empty;
        public ApplicationUser? Customer { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int Quantity { get; set; }
    }
}