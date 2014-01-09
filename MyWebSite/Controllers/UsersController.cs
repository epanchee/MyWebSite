using System;
using System.Globalization;
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
