using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MyWebSite.Models
{
    public class DataBaseReader
    {
        private static readonly TFbaseDataContext Context = new TFbaseDataContext("Server=tcp:d1pvugg7sy.database.windows.net,1433;Database=TFbase;User ID=tfsite;Password=Rewq54321;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;");

        private static String CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public static String getAllCounter(String uid)
        {
            var result = Context.Counter.First(p => p.uid == uid);
            var ret = result.count;
            Context.Connection.Close();
            return ret + "";
        }

        public static void IncCounter(String uid)
        {
            if (Context.Counter.Select(t => t.uid).Contains(uid))
            {
                var result = Context.Counter.First(p => p.uid == uid);
                result.count++;
                if (TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, TimeZoneInfo.Local.Id,
                    "Ekaterinburg Standard Time").Date == result.last.Value.Date)
                {
                    result.today++;
                }
                else
                {
                    result.today = 1;
                }
            }
            else
            {
                var record = new Counter {uid = uid, count = 1, today = 1};
                Context.Counter.InsertOnSubmit(record);
            }
            Context.SubmitChanges();
            Context.Connection.Close();
        }

        public static String GenerateSecret(String uid)
        {
            var result = Context.Counter.First(p => p.uid == uid);
            var secret = result.secret = CalculateMD5Hash(uid + DateTime.Now);
            Context.SubmitChanges();
            Context.Connection.Close();
            return secret;
        }

        public static String GetLastDate(String uid)
        {
            var result = Context.Counter.First(p => p.uid == uid);
            var ret = result.last;
            Context.Connection.Close();
            return ret + "";
        }

        public static void SetLastDate(String uid)
        {
            if (Context.Counter.Select(t => t.uid).Contains(uid))
            {
                var result = Context.Counter.First(p => p.uid == uid);
                result.last = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, TimeZoneInfo.Local.Id,
                    "Ekaterinburg Standard Time");
                Context.SubmitChanges();
            }
            else
            {
                var record = new Counter { uid = uid, count = 1, last = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, TimeZoneInfo.Local.Id,
                    "Ekaterinburg Standard Time")};
                Context.Counter.InsertOnSubmit(record);
            }
            Context.Connection.Close();
        }

        public static String GetTodayCounter(string uid)
        {
            var result = Context.Counter.First(p => p.uid == uid);
            var ret = result.today;
            Context.Connection.Close();
            return ret + "";
        }

        public static IQueryable<Reports> GetReports()
        {
            return from r in Context.Reports select r;
        }

        public static bool PostReport(String text, String uid, String secret)
        {
            if (Context.Counter.Select(t => t.uid).Contains(uid))
            {
                var r = Context.Counter.First(p => p.uid == uid);
                if (r.secret != secret) return false;
                var record = new Reports { uid = uid, content = text, date = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, TimeZoneInfo.Local.Id,
                    "Ekaterinburg Standard Time")};
                Context.Reports.InsertOnSubmit(record);
                Context.SubmitChanges();
                Context.Connection.Close();
                return true;
            }

            Context.Connection.Close();
            return false;
        }

        public static String GetReport(int id)
        {
            var r = Context.Reports.First(p => p.Id == id);
            var res = r.content;
            Context.Connection.Close();
            return res;
        }

        public static bool EditReport(string text, string secret, string uid, int id)
        {
            if (Context.Counter.Select(t => t.uid).Contains(uid))
            {
                var r = Context.Counter.First(p => p.uid == uid);
                if (r.secret != secret) return false;

                var report = Context.Reports.First(p => p.Id == id);
                report.content = text;

                Context.Connection.Close();
                return true;
            }

            Context.Connection.Close();
            return false;
        }

        public static String DeleteReport(string secret, string uid, int id)
        {
            if (Context.Counter.Select(t => t.uid).Contains(uid))
            {
                var r = Context.Counter.First(p => p.uid == uid);
                if (r.secret != secret) return "Wrong secret";

                var report = Context.Reports.First(p => p.Id == id);
                Context.Reports.DeleteOnSubmit(report);
                Context.SubmitChanges();

                Context.Connection.Close();
                return "Submited";
            }

            Context.Connection.Close();
            return "Wrong uid";
        }
    }
}