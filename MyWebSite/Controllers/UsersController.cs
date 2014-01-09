using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
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
            //var user = VKapi.Interact(Request["uid"]); 
            //return Json(new{uid = Request["uid"], secret = Request["secret"], id = Request["id"], name = user.first_name, photo = user.photo_100},
            //    JsonRequestBehavior.AllowGet); 
            return View();
        }
    }
}
