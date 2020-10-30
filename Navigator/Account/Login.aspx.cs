using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using Navigator.Dal.Concrete;
using Navigator.Helpers;
using Newtonsoft.Json;

namespace Navigator.Account
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void LogIn(object sender, EventArgs e)
        {
            if (!IsValid) return;
            var user = new UserDa().ValidateUser(Username.Text.Trim(), Password.Text.Trim());
            if (user != null)
            {
                var tkt = new FormsAuthenticationTicket(1, Username.Text.Trim(), DateTime.Now,
                    DateTime.Now.AddMinutes(60), false, JsonConvert.SerializeObject(user));
                var cookiestr = FormsAuthentication.Encrypt(tkt);
                var ck = new HttpCookie(FormsAuthentication.FormsCookieName, cookiestr)
                {
                    Path = FormsAuthentication.FormsCookiePath,
                    Expires = tkt.Expiration
                };
                Response.Cookies.Add(ck);
                var pageUrl = Request.QueryString["ReturnUrl"];
                IdentityHelper.RedirectToReturnUrl(
                    pageUrl != null ? Request.QueryString["ReturnUrl"] : "/Maps/General", Response);
            }
            else
            {
                FailureText.Text = "Грешно корисничко име или лозинка";
                ErrorMessage.Visible = true;
            }
        }
    }
}