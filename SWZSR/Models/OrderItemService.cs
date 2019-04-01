using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SWZSR.Models
{
    public class OrderItemService
    {
        public int OrderItemServiceId { get; set; }
        [Required]
        public decimal UnitPrice { get; set; }
        [Required]
        public int ServiceId { get; set; }
        [Required]
        public int OrderItemId { get; set; }

        public virtual OrderItem OrderItem { get; set; }
        public virtual Service Service { get; set; }
    }
}
