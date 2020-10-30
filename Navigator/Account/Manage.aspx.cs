using System;
using System.Web.Security;
using System.Web.UI;
using Navigator.Dal.Concrete;
using Navigator.Models.Concrete;
using Newtonsoft.Json;

namespace Navigator.Account
{
    public partial class Manage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                changePasswordHolder.Visible = true;

                // Render success message
                var message = Request.QueryString["m"];
                if (message != null)
                {
                    // Strip the query string from action
                    Form.Action = ResolveUrl("~/Account/Manage");

                    SuccessMessage =
                        message == "ChangePwdSuccess" ? "Лозинката е успешно променета" : String.Empty;
                    successMessage.Visible = !String.IsNullOrEmpty(SuccessMessage);
                }
            }
        }

        protected string SuccessMessage
        {
            get;
            private set;
        }

        protected void UpdateButton_OnClick(object sender, EventArgs e)
        {
            if (!IsValid) return;
            var id = (FormsIdentity)User.Identity;
            var user = JsonConvert.DeserializeObject<User>(id.Ticket.UserData);
            try
            {
                var result = new UserDa().ChangeUserPassword(user.UserId, CurrentPassword.Text.Trim(), NewPassword.Text.Trim());
                if (result)
                {
                    Response.Redirect("~/Account/Manage?m=ChangePwdSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "Лозинката не е променета");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }
    }
}