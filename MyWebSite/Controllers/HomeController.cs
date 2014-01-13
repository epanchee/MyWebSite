using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
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

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Rep(String a)
        {
            try
            {
                var dbreader = new DataBaseReader();
                var html = Request.Unvalidated.Form["report_text"];

                #region Фильтруем только нужные теги
                var sb = new StringBuilder(HttpUtility.HtmlEncode(html));
                sb.Replace("&lt;b&gt;", "<b>");
                sb.Replace("&lt;/b&gt;", "</b>");
                sb.Replace("&lt;i&gt;", "<i>");
                sb.Replace("&lt;/i&gt;", "</i>");
                var text = Regex.Replace(sb.ToString(), "&lt;img(?:.*?)src=&quot;(.*?)&quot;/&gt;", "<img src=\"$1\"/>");
                text = text.Replace("\"\"", "\"");
                #endregion

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
            var dbreader = new DataBaseReader();
            if (!dbreader.CheckIPinVote(Request.UserHostAddress))
            {
                return View();
            }
            return RedirectToAction("Voted");
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
                dbreader.InsertPreference(Request["pref"].Split(','), Request.UserHostAddress);
                return RedirectToAction("Voted");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }

            return RedirectToAction("Index");
        }

        public ActionResult Voted()
        {
            return View();
        }

        public ActionResult VoteResult()
        {
            return View();
        }
    }
}
