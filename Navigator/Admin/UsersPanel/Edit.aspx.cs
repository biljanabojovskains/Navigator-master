using System;
using System.Collections.Generic;
using System.Web.UI;
using Microsoft.AspNet.FriendlyUrls;
using Navigator.Bll;
using Navigator.Models.Abstract;
using NLog;

namespace Navigator.Admin.UsersPanel
{
    public partial class Edit : Page
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            var user = GetUser(GetUserIdFromUrl());
            Fill(user);
            FillUsers(user);
        }

        private int GetUserIdFromUrl()
        {
            IList<string> segments = Request.GetFriendlyUrlSegments();
            int pId = -1;
            if (segments.Count != 0)
                int.TryParse(segments[0], out pId);
            return pId;
        }

        private IUser GetUser(int userId)
        {
            var user = Bl.GetUser(userId);
            return user;
        }


        private void FillUsers(IUser user)
        {
            var roles = Bl.GetAllRoles();
            DdlRoles.DataSource = roles;
            DdlRoles.DataBind();
        }
        private void Fill(IUser user)
        {
  
            TxtUsername.Text = user.UserName;
            TxtFullname.Text = user.FullName;
            TxtPhone.Text = user.Phone;
            TxtEmail.Text = user.Email;
            CbActive.Checked = user.Active;
        }

        protected void UpdateButton_OnClick(object sender, EventArgs e)
        {
            try
            {
                var result = Bl.UpdateUser(GetUserIdFromUrl(), int.Parse(DdlRoles.SelectedValue), TxtUsername.Text, TxtFullname.Text, TxtPhone.Text,
                    TxtEmail.Text, CbActive.Checked);
                if (result)
                    Response.Redirect("../Default");
                else
                    SetErrorMessage("Корисниот не е променет");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                //SetErrorMessage("Грешка при менување на корисник");
                SetErrorMessage(ex.Message);
            }
        }

        private void SetErrorMessage(string output)
        {
            ErrorLabel.Visible = true;
            ErrorLabel.Text = output;
        }

        protected void CancelButton_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("../Default");
        }
    }
}