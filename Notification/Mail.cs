using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Notification
{
    public static class Mail
    {
        public static bool SendEmail(string fullName, DateTime datumOd, DateTime datumDo, string email, string messageText, string tema, string podtema)
        {
            bool result = false;
            MailMessage mail = new MailMessage();
            mail.SubjectEncoding = System.Text.Encoding.UTF8;
            mail.BodyEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("start to send email ...");

            // Set the to and from addresses.
            // The from address must be your GMail account


            mail.From = new MailAddress(ConfigurationManager.AppSettings["user"]);

            mail.To.Add(email);
            //  mail.To.Add("ana_barabanovska@yahoo.com");
            // mail.To.Add("anff.barabanovska@gmail.com");

            // Define the message


            mail.Subject = "Известување за точки од интерес";
            mail.IsBodyHtml = true;

            mail.Body += " <html>";
            mail.Body += "<body>";
            mail.Body += "<table style='width:100%;padding-top:-20px;'>";
            mail.Body += "<tr>";
            mail.Body += "<td style='width:60%;'>Почитуван/а " + "   " + fullName + "</td>";
            mail.Body += "</tr>";
            mail.Body += "<br/>";

            mail.Body += "<tr>";
            mail.Body += "<td>Општина Центар ве известува дека ќе има промени на точките кои ви се од интерес. </td>";
            mail.Body += "</tr>";
            mail.Body += "<tr>";
            mail.Body += "</tr>";
            mail.Body += "<tr>";
            mail.Body += "<td><b>Тема:</b>" + "   " + tema + " </td>";
            mail.Body += "</tr>";
            mail.Body += "<tr>";
            mail.Body += "</tr>";
            mail.Body += "<tr>";
            mail.Body += "<td><b>Подтема:</b>" + "   " + podtema + " </td>";
            mail.Body += "</tr>";
            // mail.Body += "<br/>";
            mail.Body += "<tr>";
            mail.Body += "</tr>";
            mail.Body += "<tr>";
            mail.Body += "<td style='width:40%;'><b>Датум кога ќе започне промената:</b>" + "   " + datumOd.ToString("dd/MM/yyyy") + "</td>";
            mail.Body += "</tr>";
            mail.Body += "<tr>";
            mail.Body += "</tr>";
            mail.Body += "<tr>";
            mail.Body += "<td><b>Датум кога ќе заврши промената:</b>" + "   " + datumDo.ToString("dd/MM/yyyy") + "</td>";
            mail.Body += "</tr>";
            mail.Body += "<tr>";
            mail.Body += "<tr>";
            mail.Body += "</tr>";
            mail.Body += "<tr>";
            mail.Body += "</tr>";
            mail.Body += "<td>" + messageText + " </td>";
            mail.Body += "</tr>";
            mail.Body += "<tr>";
            mail.Body += "</tr>";
            mail.Body += "<tr>";
            mail.Body += "<td><br/>Со почит,</td></tr>";
            mail.Body += "<tr><td><br/>Општина Центар </td>";
            mail.Body += "</tr>";
            mail.Body += "</table>";
            mail.Body += "</body>";
            mail.Body += "</html>";

            // Create a new Smpt Client using Google's servers
            var SmtpServer = new SmtpClient();

            SmtpServer.Host = ConfigurationManager.AppSettings["SmtpServer"]; //for gmail
            SmtpServer.Port = Convert.ToInt32(ConfigurationManager.AppSettings["port"]); //for gmail
            SmtpServer.EnableSsl = true; //ForGmail
            SmtpServer.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["user"],
                                                              ConfigurationManager.AppSettings["pass"]);

            try
            {
                SmtpServer.Send(mail);
                SmtpServer.Dispose();

                //    mail.DeliveryNotificationOptions = System.Net.Mail.DeliveryNotificationOptions.OnSuccess;

                Console.WriteLine("email was sent successfully!");
                result = true;
            }
            catch (SmtpFailedRecipientsException ex)
            {
                for (int i = 0; i < ex.InnerExceptions.Length; i++)
                {
                    SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                    if (status == SmtpStatusCode.MailboxBusy || status == SmtpStatusCode.MailboxUnavailable)
                    {
                        Console.WriteLine("Delivery failed - retrying in 5 seconds.");
                        System.Threading.Thread.Sleep(5000);
                        SmtpServer.Send(mail);
                        result = true;
                    }
                    else
                    {
                        Console.WriteLine("Failed to deliver message to {0}", ex.InnerExceptions[i].FailedRecipient);
                        result = false;
                        throw ex;
                    }
                }
            }
            catch (Exception ep)
            {
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ep.Message);
                result = false;

            }
            finally
            {
                SmtpServer.Dispose();
            }
            return result;
        }

    }
}
