using Microsoft.AspNetCore.Mvc;
using SWZSR.Infrastructure;
using SWZSR.Infrastructure.Alerts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWZSR.ViewComponents
{
    public class AlertsViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            //var alerts = TempData.ContainsKey(Alert.TempDataKey)
            //    ? (List<Alert>)TempData[Alert.TempDataKey]
            //    : new List<Alert>();
            var alerts = TempData.DeserializeAlerts(Alert.TempDataKey);

            return View(alerts);
        }
    }
}
