using SWZSR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWZSR.ViewModels
{
    public class OrderListViewModel
    {
        public IList<Order> Orders { get; set; }
    }
}
