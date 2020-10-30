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
    public partial class Forgot : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

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
        protected void Forgot_Click(object sender, EventArgs e)
        {
            var result = Bl.CreateResetPasswordToken(Email.Text);
            if (!result)
            {
                DisplayErrorMessage("Корисникот не постои");
                return;
            }
            HideLoginForm();
        }

        private void DisplayErrorMessage(string error)
        {
            FailureText.Text = error;
            ErrorMessage.Visible = true;
        }

        private void HideLoginForm()
        {
            loginForm.Visible = false;
            DisplayEmail.Visible = true;
        }
    }
}