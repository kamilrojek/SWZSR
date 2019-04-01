using SWZSR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWZSR.ViewModels
{
    public class OrderViewModel
    {
        public Order Order { get; set; }
        public IList<Service> Services { get; set; }
    }
}
