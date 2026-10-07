namespace Final_Project.ViewModels.Review
{
    public class ReviewListItemViewModel
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
