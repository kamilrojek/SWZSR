using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWZSR.Infrastructure.Alerts
{
    public class AlertService
    {
        private readonly ITempDataDictionary _tempData;

        public AlertService(IHttpContextAccessor contextAccessor, ITempDataDictionaryFactory tempDataDictionaryFactory)
        {
            _tempData = tempDataDictionaryFactory.GetTempData(contextAccessor.HttpContext);
        }

        public void Success(string message, bool dismissable = false)
        {
            AddAlert(AlertStyles.Success, message, dismissable);
        }

        public void Danger(string message, bool dismissable = false)
        {
            AddAlert(AlertStyles.Danger, message, dismissable);
        }

        public void Warning(string message, bool dismissable = false)
        {
            AddAlert(AlertStyles.Warning, message, dismissable);
        }

        private void AddAlert(string alertStyle, string message, bool dismissable)
        {
            //var alerts = _tempData.ContainsKey(Alert.TempDataKey)
            //    ? (List<Alert>)_tempData[Alert.TempDataKey]
            //    : new List<Alert>();
            var alerts = _tempData.DeserializeAlerts(Alert.TempDataKey);

            alerts.Add(new Alert
            {
                AlertStyle = alertStyle,
                Messege = message,
                Dismissable = dismissable
            });

            _tempData.SerializeAlerts(Alert.TempDataKey, alerts);
            //_tempData[Alert.TempDataKey] = alerts;
        }
    }
}
