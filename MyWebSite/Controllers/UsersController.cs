using System;
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using MyWebSite.Models;

namespace MyWebSite.Controllers
{
    public class UsersController : Controller
    {
        public ActionResult Counter()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
            var uid = Request["uid"];
            var dbreader = new DataBaseReader();
            dbreader.IncCounter(uid);
            var last_date = dbreader.GetLastDate(uid);
            dbreader.SetLastDate(uid);
            try
            {
                var wb = new WebClient { Encoding = Encoding.UTF8 };
                var all = wb.DownloadString(String.Format("http://146.185.176.129/help/image?resolution=50x50&text1=1,15,{0}", dbreader.getAllCounter(uid)));
                var today = wb.DownloadString(String.Format("http://146.185.176.129/help/image?resolution=50x50&text1=1,15,{0}", dbreader.GetTodayCounter(uid)));
                var all_counter = String.Format("<img src=\"data:image/png;base64,{0}\" />", all);
                var today_counter = String.Format("<img src=\"data:image/png;base64,{0}\" />", today);
                return Json(new { secret = dbreader.GenerateSecret(uid), count = all_counter, last = last_date, today = today_counter }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                
            }
            return Json(new {secret = dbreader.GenerateSecret(uid), count = dbreader.getAllCounter(uid), last = last_date, today = dbreader.GetTodayCounter(uid)}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Time()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
            return Json(new
            {
                now = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, TimeZoneInfo.Local.Id,
                    "Ekaterinburg Standard Time").ToString("G")
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditReport()
        {
            return View();
        }

        public ActionResult Voting()
        {
            return View();
        }

    }
}
