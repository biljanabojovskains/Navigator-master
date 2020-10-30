using System;
using System.Configuration;
using System.Linq;
using System.Web.Services;
using System.Web.UI;
using Navigator.Bll;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Navigator.Modules.Stats
{
    public partial class Stats1 : System.Web.UI.Page
    {
        protected override void OnPreLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                var modules = ConfigurationManager.AppSettings["mods"].Split(',');
                var modul = modules.Contains("s1pN1");
                if (!modul)
                {
                    Response.Redirect("~/NoAccess");
                }
            }
            base.OnPreLoad(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {  
            FillProjects();
        }

        private void FillProjects()
        {
            var projects = Bl.GetAllTekovniOpfati();
            selectProekt.DataSource = projects;
            selectProekt.DataTextField = "Ime";
            selectProekt.DataValueField = "Id";
            selectProekt.DataBind();
        }

        [WebMethod]
        public static string ChangeProject(int id)
        {
            var item = Bl.GetNamena(id);
            var result = JsonConvert.SerializeObject(item);
            return result;
        }
        [WebMethod]
        public static string Search(int projId)
        {
            var item = Bl.GetProjectStat(projId);
            var result = JsonConvert.SerializeObject(item);
            return result;
        }

        [WebMethod]
        public static string SearchNamena(string namena, int opfatId)
        {
            var item = Bl.GetNamenaStat(namena, opfatId);   
            var result = JsonConvert.SerializeObject(item);
            return result;
        }
       
    }
}