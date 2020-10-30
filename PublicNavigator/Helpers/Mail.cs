using System;
using System.Net;
using System.Net.Mail;
using NLog;
using System.Configuration;

namespace PublicNavigator.Helpers
{
    public static class Mail
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static bool SendMail(string strTo, string from, string subject, string strBody)
        {
            var myMail = new MailMessage();
            var sc = new SmtpClient();
            myMail.From = new MailAddress(from, from);
            myMail.To.Add(new MailAddress(strTo, strTo));
            myMail.Subject = subject;
            myMail.Priority = MailPriority.Normal;
            myMail.IsBodyHtml = true;
            myMail.Body = strBody;
            sc.Host = ConfigurationManager.AppSettings["mailServer"];
            sc.Port = Convert.ToInt32(ConfigurationManager.AppSettings["mailPort"]);
            sc.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["mailUser"],
                ConfigurationManager.AppSettings["mailPass"]);
            sc.EnableSsl = true;
            try
            {
                sc.Send(myMail);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }
    }
}