using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using Navigator.Bll;
using Navigator.Models.Concrete;
using Newtonsoft.Json;

namespace Navigator.Modules.Notifikacii
{
    public partial class Insert : Page
    {
        protected override void OnPreLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                var modules = ConfigurationManager.AppSettings["mods"].Split(',');
                var modul = modules.Contains("t1pN1");
                if (!modul)
                {
                    Response.Redirect("~/NoAccess");
                }
            }
            base.OnPreLoad(e);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            FillThemes();
        }
        private void FillThemes()
        {
            var temi = Bl.GetAllTemi();
            selectTema.DataSource = temi;
            selectTema.DataTextField = "ImeTema";
            selectTema.DataValueField = "Id";
            selectTema.DataBind();
        }
        [WebMethod]
        public static string FillSubThemes(int id)
        {
            var item = Bl.GetAllPodTemi(id);
            var result = JsonConvert.SerializeObject(item);
            return result;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            var dateOd = DateTime.ParseExact(datumOd.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var dateDo = DateTime.ParseExact(datumDo.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            string podtemaId = fkPodtema.Value;
            int fk_Podtema = Convert.ToInt32(podtemaId);
            string koordinati = coordinates.Value;
            var id = (FormsIdentity)User.Identity;
            var user = JsonConvert.DeserializeObject<User>(id.Ticket.UserData);
            int userId = user.UserId;

            var insert = Bl.InsertNotifikacija(fk_Podtema, userId, komentar.Text, dateOd.Date, dateDo.Date, koordinati);
            datumOd.Text = "";
            datumDo.Text = "";
            komentar.Text = "";
            Response.Redirect("Default");
        }
    }
}
