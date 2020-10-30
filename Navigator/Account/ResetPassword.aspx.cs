using System;
using System.Web.UI;
using Navigator.Bll;

namespace Navigator.Account
{
    public partial class ResetPassword : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
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