namespace FoodSquad_API.Models.DTO.Order
{
    public class OrderDTO
    {
        public Guid Id { get; set; }
        public Dictionary<long, int> MenuItemQuantities { get; set; }
        public string Status { get; set; }
        public double TotalCost { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool Paid { get; set; }
        public string UserEmail { get; set; }

        public OrderDTO(Guid id, string userEmail, Dictionary<long, int> menuItemQuantities, string status, double totalCost, DateTime createdOn, bool paid)
        {
            Id = id;
            UserEmail = userEmail;
            MenuItemQuantities = menuItemQuantities;
            Status = status;
            TotalCost = totalCost;
            CreatedOn = createdOn;
            Paid = paid;
        }

        // Default constructor
        public OrderDTO() { }
    }
}
