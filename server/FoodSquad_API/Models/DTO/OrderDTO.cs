using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FoodSquad_API.Models.DTO
{
    public class OrderDTO
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Menu item quantities are required")]
        public Dictionary<long, int> MenuItemQuantities { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; }

        public double TotalCost { get; set; }

        [Required(ErrorMessage = "Creation date is required")]
        public DateTime CreatedOn { get; set; }

        public bool Paid { get; set; }

        public string UserEmail { get; set; }

        public OrderDTO() { }

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
    }
}
