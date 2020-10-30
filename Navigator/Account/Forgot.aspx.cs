using System;
using System.Web.UI;
using Navigator.Bll;

namespace Navigator.Account
{
    public partial class Forgot : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

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