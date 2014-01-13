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
            var uid = Request["uid"];
            var dbreader = new DataBaseReader();
            return Json(new {secret = dbreader.GenerateSecret(uid)}, JsonRequestBehavior.AllowGet);
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
