namespace Final_Project.ViewModels.Admin
{
    public class AdminDashboardDto
    {
        public int TotalCustomers { get; set; }
        public int TotalSellers { get; set; }
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
    }

    public class SellerDashboardDto
    {
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSales { get; set; }
    }
}