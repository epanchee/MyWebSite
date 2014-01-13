using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;

namespace MyWebSite.Models
{
    public class DataBaseReader
    {
        private readonly TFbaseDataContext Context = new TFbaseDataContext("Server=tcp:d1pvugg7sy.database.windows.net,1433;Database=TFbase;User ID=tfsite;Password=Rewq54321;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;");

        private String CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public String GenerateSecret(String uid)
        {
            var result = Context.Counter.First(p => p.uid == uid);
            var secret = result.secret = CalculateMD5Hash(uid + DateTime.Now);
            Context.SubmitChanges();
            Context.Connection.Close();
            return secret;
        }

        public List<Reports> GetReports()
        {
            var ret = Context.Reports.ToList();
            Context.Connection.Close();
            return ret;
        }

        public bool PostReport(String text, String uid, String secret)
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

        public String GetReport(int id)
        {
            var r = Context.Reports.First(p => p.Id == id);
            var res = r.content;
            Context.Connection.Close();
            return res;
        }

        public bool EditReport(string text, string secret, string uid, int id)
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

        public String DeleteReport(string secret, string uid, int id)
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

        public bool InsertVote(Voting record, string secret, string ip)
        {
            if (!Context.Voting.Select(t => t.ip).Contains(ip))
            {
                Context.Voting.InsertOnSubmit(record);
                Context.SubmitChanges();
                Context.Connection.Close();
                return true;
            }

            Context.Connection.Close();
            return false;
        }

        public int[] GetRateVotesCount()
        {
            var ret = new int[] {0,0,0,0,0};
            for (var i = 1; i<=5; i++)
            {
                ret[i-1] = Context.Voting.Count(t => t.rate == i);
            }
            Context.Connection.Close();
            return ret;
        }

        public bool CheckIPinVote(string ip)
        {
            return Context.Voting.Select(t => t.ip).Contains(ip);
        }

        public bool InsertPreference(string [] prefs, string _name)
        {
            var flag = false;
            foreach (var pref in prefs)
            {
                Context.PrefMusic.InsertOnSubmit(new PrefMusic() {vote_ip = _name, style = pref});
                if (!flag) flag = true;
            }
            Context.SubmitChanges();
            Context.Connection.Close();
            return flag;
        }

        public List<Voting> GetAllVotes()
        {
            var ret = Context.Voting.ToList();
            Context.Connection.Close();
            return ret;
        }

        public List<PrefMusic> GetAllPreference(String ip)
        {
            var ret = Context.PrefMusic.Where(t => t.vote_ip == ip).ToList();
            Context.Connection.Close();
            return ret;
        }

        public int IPgetAllCounter()
        {
            var ret = Context.ip_counter.Count();
            Context.Connection.Close();
            return ret;
        }

        public int IPGetTodayCounter()
        {
            var ret =
                Context.ip_counter.Count(
                    t =>
                        t.last_visit.Value.Day ==
                        TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, TimeZoneInfo.Local.Id,
                            "Ekaterinburg Standard Time").Day);
            Context.Connection.Close();
            return ret;
        }

        public void IPSetLastDate(string ip, DateTime? lastUsing)
        {
            var rec = Context.ip_counter.First(t => t.ip == ip);
            rec.last_visit = lastUsing;
            Context.Connection.Close();
        }

        public DateTime? IPGetLastDate(string ip)
        {
            DateTime? ret = null;

            if (Context.ip_counter.Any(t => t.ip == ip))
            {
                ret = Context.ip_counter.First(t => t.ip == ip).last_visit.Value;
                Context.Connection.Close();
            }
            else
            {
                var record = new ip_counter
                {
                    ip = ip,
                    last_using = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, TimeZoneInfo.Local.Id,
                    "Ekaterinburg Standard Time"),
                    last_visit = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, TimeZoneInfo.Local.Id,
                    "Ekaterinburg Standard Time")
                };
                Context.ip_counter.InsertOnSubmit(record);
                Context.SubmitChanges();
            }

            return ret;
        }

        public DateTime? IPGetLastUsingDate(string ip)
        {
            var ret = Context.ip_counter.First(t => t.ip == ip).last_using;
            Context.Connection.Close();
            return ret;
        }

        public void IPSetLastUsingDate(string ip)
        {
            var rec = Context.ip_counter.First(t => t.ip == ip);
            rec.last_visit = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, TimeZoneInfo.Local.Id,
                            "Ekaterinburg Standard Time");
            Context.Connection.Close();
        }
    }
}