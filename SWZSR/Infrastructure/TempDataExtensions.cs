using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using SWZSR.Infrastructure.Alerts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWZSR.Infrastructure
{
    public static class TempDataExtensions
    {
        public static void SerializeAlerts(this ITempDataDictionary tempData, string alertKeyName, List<Alert> alerts)
        {
            tempData[alertKeyName] = JsonConvert.SerializeObject(alerts);
        }

        public static List<Alert> DeserializeAlerts(this ITempDataDictionary tempData, string alertKeyName)
        {
            var alerts = new List<Alert>();
            if (tempData.ContainsKey(alertKeyName))
            {
                alerts = JsonConvert.DeserializeObject<List<Alert>>(tempData[alertKeyName].ToString());
            }
            return alerts;
        }
    }
}
