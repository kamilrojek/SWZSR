using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SWZSR.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public string ItemPhoto { get; set; }
        [Required]
        public string ItemModel { get; set; }
        [Required]
        public string ItemColour { get; set; }
        public string Comment { get; set; }
        [Required]
        public int OrderId { get; set; }
        public string MechanicId { get; set; }

        public virtual Order Order { get; set; }
        public virtual List<OrderItemService> OrderItemServices { get; set; }
    }
}
