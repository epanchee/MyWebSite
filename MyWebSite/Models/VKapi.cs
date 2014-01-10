using System;
using System.Net;
using System.Text;
using System.Web.Helpers;

namespace MyWebSite.Models
{
    public static class VKapi
    {
        public static dynamic Interact(String uid)
        {
            var wb = new WebClient();
            wb.Encoding = Encoding.UTF8;
            try
            {
                var response = wb.DownloadString("https://api.vk.com/method/getProfiles?uid=" + uid + "&fields=photo_100");
                return Json.Decode(response.Substring(response.IndexOf('[') + 1, (response.IndexOf(']') - response.IndexOf('[') - 1)));
            }
            catch (Exception)
            {
            }

            return Json.Encode(new {firstname = "error"});
        }
    }
}