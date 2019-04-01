using SWZSR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWZSR.ViewModels
{
    public class WorkshopViewModel
    {
        public OrderItem OrderItem { get; set; }
        public IList<Service> Services { get; set; }
    }

    public class WorkshopCalendarViewModel
    {
        public string Title { get; set; }
        public string Start { get; set; }
        public string Url { get; set; }
        public string Color { get; set; }
        public string TextColor { get; set; }
    }
}
