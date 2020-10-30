using System;
using System.Configuration;
using System.Linq;
using System.Web.Services;
using System.Web.UI;
using Navigator.Bll;
using Newtonsoft.Json;

namespace Navigator.Modules.Preklop
{
    public partial class Mikro : Page
    {
        protected override void OnPreLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                var modules = ConfigurationManager.AppSettings["mods"].Split(',');
                var modul = modules.Contains("m1pN1");
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
            var projects = Bl.GetOpfatiNedoneseni();
            //selectProekt.DataSource = projects;
            //selectProekt.DataTextField = "Ime";
            //selectProekt.DataValueField = "Id";
            //selectProekt.DataBind();
            var datasource = from x in projects
                             select new
                             {
                                 x.Id,
                                 x.Ime,
                                 x.DatumNaDonesuvanje,
                                 DisplayField = String.Format("{0} ({1})", x.Ime, x.DatumNaDonesuvanje.Value.ToString("dd.MM.yyyy"))
                             };

            selectProekt.DataSource = datasource;
            selectProekt.DataValueField = "Id";
            selectProekt.DataTextField = "DisplayField";
            selectProekt.DataBind();
        }

        [WebMethod]
        public static string ChangeProject(int id)
        {
            var item = Bl.GetOpfat(id);
            var result = JsonConvert.SerializeObject(item);
            return result;
        }

        [WebMethod]
        public static string Search(int projId)
        {
            var item = Bl.GeneratePreklop(projId);
            var result = JsonConvert.SerializeObject(item);

            return result;
        }
        [WebMethod]
        public static string DownloadExcel(string ids)
        {
            //HttpContext.Current.Response.SetCookie(new HttpCookie("fileDownload", "true") { Path = "/" });
            //var fileName = _presenter.OnDownloadClicked(ids);
            //return fileName;
            return "dare";
        }
    }
}