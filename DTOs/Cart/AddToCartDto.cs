namespace Final_Project.DTOs.Cart
{
	public class AddToCartDto
	{
		public int ProductId { get; set; }
		public int Quantity { get; set; } = 1;
	}
}