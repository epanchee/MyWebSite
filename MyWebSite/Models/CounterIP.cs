using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace MyWebSite.Models
{
    public static class CounterIP
    {
        public static dynamic Count(string ip)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
            var dbreader = new DataBaseReader();
            var lst = dbreader.IPGetLastDate(ip);
            var last_date = "Никогда";

            if (lst == null)
            {
                dbreader.IPSetLastUsingDate(ip);
            }
            else
            {
                last_date = lst.Value.ToString("G");
            }

            var last_using = dbreader.IPGetLastUsingDate(ip);
            if (last_using == null || (TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, TimeZoneInfo.Local.Id,
                    "Ekaterinburg Standard Time") - last_using.Value).Minutes > 10)
            {
                dbreader.IPSetLastDate(ip, last_using);
            }
            dbreader.IPSetLastUsingDate(ip);

            string allbase64 = "", todaybase64 = "";
            int all = 0, today = 0; 

            try
            {
                var wb = new WebClient { Encoding = Encoding.UTF8 };
                all = dbreader.IPgetAllCounter();
                today = dbreader.IPGetTodayCounter();
                allbase64 = wb.DownloadString(String.Format("http://146.185.176.129/help/image?resolution=50x30&text1=1,15,{0}", all));
                todaybase64 = wb.DownloadString(String.Format("http://146.185.176.129/help/image?resolution=50x30&text1=1,15,{0}", today));
            }
            catch (Exception)
            {

            }

            var all_counter = String.Format("<img alt=\"{0}\" src=\"data:image/png;base64,{1}\" />", all, allbase64);
            var today_counter = String.Format("<img alt=\"{0}\" src=\"data:image/png;base64,{1}\" />", today, todaybase64);

            return new { last = last_date, allcnt = all_counter, todcnt = today_counter};
        }
    }
}