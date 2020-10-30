using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Navigator.Bll;

namespace Navigator.Admin.UsersPanel
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            GetUsers();
        }

        private void GetUsers()
        {
            var users = Bl.GetAllUsers();
            ViewState["listUsers"] = users;
            BindListView();
        }

        protected void ListUsers_OnPagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
        {
            //set current page startindex, max rows and rebind to false
            DataPager1.SetPageProperties(e.StartRowIndex, e.MaximumRows, false);

            //rebind List View
            BindListView();
        }

        private void BindListView()
        {
            ListUsers.DataSource = ViewState["listUsers"];
            ListUsers.DataBind();
        }
    }
}