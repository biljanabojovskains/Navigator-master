using PublicNavigator.Bll;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PublicNavigator.Account
{
    public partial class ResetPassword : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
        }
        protected override void OnPreLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                var modules = ConfigurationManager.AppSettings["mods"].Split(',');
                var modul = modules.Contains("n1pN1");
                if (!modul)
                {
                    Response.Redirect("~/NoAccess");
                }
            }
            base.OnPreLoad(e);
        }
        protected void Reset_Click(object sender, EventArgs e)
        {
            var code = Request.QueryString["token"];
            if (code != null)
            {
                var result = Bl.ResetPassword(Email.Text, code, Password.Text.Trim());
                if (result)
                {
                    RedirectToLoginPage();
                    return;
                }

                DisplayErrorMessage("Настаната е грешка");
                return;
            }
            DisplayErrorMessage("Настаната е грешка");
        }

        private void RedirectToLoginPage()
        {
            Response.Redirect("~/Account/Login");
        }
        private void DisplayErrorMessage(string error)
        {
            ErrorMessage.Text = "Настаната е грешка";
        }
    }
}