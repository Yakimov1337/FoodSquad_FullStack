using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FoodSquad_API.Models.DTO.Order
{
    public class OrderUpdateDTO
    {
        [Required(ErrorMessage = "Menu item quantities are required")]
        public Dictionary<long, int> MenuItemQuantities { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; }

        public bool Paid { get; set; }
    }
}
