using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Navigator.Bll;

namespace Navigator.Admin.UsersPanel
{
    public partial class Logs : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            GetLogs();
        }

        private void GetLogs()
        {
            var logs = Bl.GetAllLogs();
            ViewState["listLogs"] = logs;
            BindListView();
        }

        protected void ListLogs_OnPagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
        {
            //set current page startindex, max rows and rebind to false
            DataPager1.SetPageProperties(e.StartRowIndex, e.MaximumRows, false);

            //rebind List View
            BindListView();
        }

        private void BindListView()
        {
            ListLogs.DataSource = ViewState["listLogs"];
            ListLogs.DataBind();
        }
        protected void Btnprezemi_OnCommand(object sender, CommandEventArgs e)
        {
            var path =  e.CommandArgument.ToString();
            Response.Redirect("~/Izvodi/" + path);
        }
    }
}