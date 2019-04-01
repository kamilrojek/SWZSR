using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SWZSR.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public string Comment { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateDelivered { get; set; }
        public DateTime DateCompleted { get; set; }
        public OrderState OrderState { get; set; }
        public decimal TotalPrice { get; set; }

        public virtual List<OrderItem> OrderItems { get; set; }
        public virtual ApplicationUser User { get; set; }
    }

    public enum OrderState
    {
        New,
        Accepted,
        InProgress,
        Completed,
        Received
    }
}
