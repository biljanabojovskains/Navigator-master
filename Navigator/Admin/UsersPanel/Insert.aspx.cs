using System;
using System.Web.UI;
using Navigator.Bll;
using NLog;

namespace Navigator.Admin.UsersPanel
{
    public partial class Insert : Page
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            GetRoles();
        }

        private void GetRoles()
        {
            var roles = Bl.GetAllRoles();
            DdlRoles.DataSource = roles;
            DdlRoles.DataBind();
        }
        
        protected void BtnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                var result = Bl.InsertUser(int.Parse(DdlRoles.SelectedValue), TxtUsername.Text, TxtPassword.Text,
                    TxtFullname.Text, TxtPhone.Text, TxtEmail.Text);
                if (result)
                    Response.Redirect("Default");
                else
                    SetErrorMessage("Корисниот не е внесен");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                //SetErrorMessage("Грешка при внесување на корисник");
                SetErrorMessage(ex.Message);
            }
        }

        protected void BtnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default");
        }

        private void SetErrorMessage(string output)
        {
            ErrorLabel.Visible = true;
            ErrorLabel.Text = output;
        }
    }
}