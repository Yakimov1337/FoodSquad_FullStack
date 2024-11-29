using FoodSquad_API.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodSquad_API.Models.Entity
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public double TotalCost { get; set; }

        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public bool Paid { get; set; } = false;

        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public User User { get; set; }

        [InverseProperty("Order")]
        public List<OrderMenuItem> MenuItemsWithQuantity { get; set; } = new();
    }

    public class OrderMenuItem
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Order")]
        public Guid OrderId { get; set; }
        public Order Order { get; set; }

        [ForeignKey("MenuItem")]
        public long MenuItemId { get; set; }
        public MenuItem MenuItem { get; set; }

        public int Quantity { get; set; }
    }
}
