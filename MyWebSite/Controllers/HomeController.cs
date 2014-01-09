using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyWebSite.Models;

namespace MyWebSite.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Contacts()
        {
            return View();
        }

        public ActionResult News()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Rep()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Rep(String a)
        {
            try
            {
                var dbreader = new DataBaseReader();
                var text = Request["report_text"];
                var uid = Request["uid"];
                var secret = Request["secret"];
                var type = Request["type"];
                var id = Request["id"];
                var res = "empty";
                if (type == "edit") {res = dbreader.EditReport(text, secret, uid, Int32.Parse(id)).ToString();}
                if (type == "send") {res = dbreader.PostReport(text, uid, secret).ToString();}
                if (type == "delete") {res = dbreader.DeleteReport(secret, uid, Int32.Parse(id)).ToString(); }
                //return Json(new { result = res, id = uid, content = text, typ = type, secrt = secret }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                //return Json(new { result = e.StackTrace }, JsonRequestBehavior.AllowGet);
            }
            return RedirectToAction("rep");
        }

        public ActionResult Multimedia()
        {
            return View();
        }

        public ActionResult Bio()
        {
            return View();
        }

        public ActionResult Concerts()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Vote()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Vote(String a)
        {
            try
            {
                var dbreader = new DataBaseReader();
                int rt;
                Int32.TryParse(Request["mark"], out rt);
                dbreader.InsertVote(
                    new Voting
                    {
                        ip = Request.UserHostAddress,
                        name = Request["name"],
                        email = Request["email"],
                        rate = rt,
                        text = Request["text"]
                    }, Request["secret"], Request.UserHostAddress);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }

            return RedirectToAction("Index");
        }
    }
}
