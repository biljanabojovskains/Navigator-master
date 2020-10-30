using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using Newtonsoft.Json;
using NLog;
using PublicNavigator.Bll;
using PublicNavigator.Dal.Concrete;
using PublicNavigator.Helpers;
using System.Threading;

namespace PublicNavigator.Account
{
    public partial class Register : Page
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected override void OnPreLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                var modules = ConfigurationManager.AppSettings["mods"].Split(',');
                var modul = modules.Contains("r1pN1");
                if (!modul)
                {
                    Response.Redirect("~/NoAccess");
                }
            }
            base.OnPreLoad(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void BtnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                var result = Bl.InsertUser(TxtUsername.Text.Trim(), TxtPassword.Text.Trim(), TxtFullname.Text.Trim(),
                    TxtPhone.Text.Trim(), TxtEmail.Text.Trim());
                if (result)
                {
                    var user = new UserDa().ValidateUser(TxtUsername.Text.Trim(), TxtPassword.Text.Trim());
                    if (user != null)
                    {
                        var tkt = new FormsAuthenticationTicket(1, TxtUsername.Text.Trim(), DateTime.Now,
                            DateTime.Now.AddMinutes(60), false, JsonConvert.SerializeObject(user));
                        var cookiestr = FormsAuthentication.Encrypt(tkt);
                        var ck = new HttpCookie(FormsAuthentication.FormsCookieName, cookiestr)
                        {
                            Path = FormsAuthentication.FormsCookiePath,
                            Expires = tkt.Expiration
                        };
                        

                       // string msgBody = String.Format("Почитувани,"+"<br/>" + "Успешно се регистриравте на ГИС порталот." );

                        //CENTAR
                        string msgBody = String.Format("Почитувани," + "<br/>" + "Успешно се регистриравте на ГИС порталот на Општина Центар, но за да добивате известување за одржување на јавна презентација и јавна анкета на ДУП, треба да означите точки кои ви се од интерес. Деталното објаснување како да го сторите тоа, се содржи во Упатството за пристапување до податоци и информациии достапни на ГИС апликацијата на Општина Центар.");
                        Mail.SendMail(TxtEmail.Text.Trim(), ConfigurationManager.AppSettings["mailUser"],
                            "Успешна регистрација", msgBody);

                        Response.Cookies.Add(ck);
                        var pageUrl = Request.QueryString["ReturnUrl"];
                        IdentityHelper.RedirectToReturnUrl(
                            pageUrl != null ? Request.QueryString["ReturnUrl"] : "~/", Response);
                    }
                    else
                    {
                        SetErrorMessage("Грешно корисничко име или лозинка");
                    }
                }
                else
                    SetErrorMessage("Корисниот не е внесен");
            }
            //catch (System.Threading.ThreadAbortException)
            //{
            //    }
            catch (Exception ex)
            {
                Logger.Error(ex);
                //SetErrorMessage("Грешка при внесување на корисник");
                SetErrorMessage(ex.Message);
            }
        }

        protected void BtnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Default.aspx");
        }

        private void SetErrorMessage(string output)
        {
            ErrorLabel.Visible = true;
            ErrorLabel.Text = output;
        }
    }
}