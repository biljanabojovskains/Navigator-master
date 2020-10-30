using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using Newtonsoft.Json;
using PublicNavigator.Bll;
using PublicNavigator.Models.Concrete;

namespace PublicNavigator
{
    public partial class _Default : Page
    {
        protected override void OnPreLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                var modules = ConfigurationManager.AppSettings["opstina"].Split(',');
                var modulP = modules.Contains("ПРИЛЕП");
                var modulK = modules.Contains("КАВАДАРЦИ");
                var modulAerodrom = modules.Contains("АЕРОДРОМ");
                var modulKumanovo = modules.Contains("КУМАНОВО");
                var module = ConfigurationManager.AppSettings["mods"].Split(',');
                var modulA = module.Contains("a1pN1");
                if (modulP || modulK || modulAerodrom || modulKumanovo)
                {
                    btnTocki.Visible = false;
                }
                if (modulP || modulK)
                {
                    btnDownload.Visible = false;
                }
                if (modulA)
                {
                    adresi.Attributes.CssStyle[HtmlTextWriterStyle.Visibility] = "visible";
                }
                if (modulAerodrom)
                {
                    btnInfoBiznis.Visible = true;
                    btnPolygon.Visible = true;
                    btnLine.Visible = true;
                }
                if (modulKumanovo) {
                    btnPolygon.Visible = true;
                    btnLine.Visible = true;
                }
                else
                {
                    btnInfoBiznis.Visible = false;
                    btnPolygon.Visible = false;
                    btnLine.Visible = false;
                }
            }
            base.OnPreLoad(e);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            var modules = ConfigurationManager.AppSettings["mods"].Split(',');
            var modulA = modules.Contains("a1pN1");
            if (modulA)
            {
                FillStreets();
            }
        }
        private void FillStreets()
        {
            var ulica = Bl.GetAllStreets();
            selectUlica.DataSource = ulica;
            selectUlica.DataTextField = "text";
            selectUlica.DataValueField = "id";
            selectUlica.DataBind();
        }

        [WebMethod]
        public static string FillNumbers(string ulica)
        {
            var item = Bl.GetAllNumbers(ulica);
            var result = JsonConvert.SerializeObject(item);
            return result;
        }

        [WebMethod]
        public static string SearchOutA(string ulica, string broj)
        {
            var item = Bl.SearchAdresa(ulica, broj);
            var json = JsonConvert.SerializeObject(item);
            return json;
        }
        [WebMethod]
        public static string FillThemes()
        {
            var temi = Bl.GetAllTemi();
            var result = JsonConvert.SerializeObject(temi);
            return result;
        }

        [WebMethod]
        public static string FillSubThemes(int id)
        {
            var item = Bl.GetAllPodTemi(id);
            var result = JsonConvert.SerializeObject(item);
            return result;
        }

        [WebMethod]
        public static string InfoTool(string coordinates)
        {
            var item = Bl.GetInfo(coordinates);
            string output = JsonConvert.SerializeObject(item);
            return output;
        }
        [WebMethod]
        public static string InfoBiznisTool(string coordinates)
        {
            var item = Bl.GetBiznisInfo(coordinates);
            string output = JsonConvert.SerializeObject(item);
            return output;
        }
        [WebMethod]
        public static string CheckUser()
        {
            var modules = ConfigurationManager.AppSettings["opstina"].Split(',');
            var modulAerodrom = modules.Contains("АЕРОДРОМ");
            bool userStatus;
            if (modulAerodrom)
            {
                userStatus = true;
            }
            else
            {
                userStatus = (HttpContext.Current.User != null) && HttpContext.Current.User.Identity.IsAuthenticated;
            }

            return userStatus.ToString();
        }

        [WebMethod]
        public static string SearchOut(string searchString)
        {
            if (searchString.Contains("quotes"))
            {
                searchString = searchString.Replace("quotes", "\"");
            }
            var item = Bl.SearchAll(searchString);
            var json = JsonConvert.SerializeObject(item);
            return json;
        }

        [WebMethod]
        public static string SearchOutK(string searchString)
        {
            var item = Bl.SearchKatastarskiParceli(searchString);
            var json = JsonConvert.SerializeObject(item);
            return json;
        }

        [WebMethod]
        public static bool Save(string tema, string podtema, string koordinati)
        {
            var id = (FormsIdentity) HttpContext.Current.User.Identity;
            var user = JsonConvert.DeserializeObject<User>(id.Ticket.UserData);
            int userId = user.UserId;
            int podtemaId = Convert.ToInt32(podtema);
            return Bl.InsertPoint(podtemaId, userId, koordinati,user.Email);
        }

        [WebMethod]
        public static string GenerateDxfFile(string coordinates)
        {
            var modules = ConfigurationManager.AppSettings["opstina"].Split(',');
            var modul= modules.Contains("АЕРОДРОМ");
            if (modul)
            {
                var dxfnearby = Bl.GenerateDxfNearby(coordinates);
                //var dxfnearby = Bl.GenerateDxfFile(coordinates);
                return dxfnearby;
            }
            else
            {
                var id = (FormsIdentity)HttpContext.Current.User.Identity;
                var user = JsonConvert.DeserializeObject<User>(id.Ticket.UserData);
                int userId = user.UserId;
                var dxf = Bl.GenerateDxf(coordinates, userId);
                return dxf;
            }
        }        
        [WebMethod]
        public static string GenerateOpstiUslovi(int opstiId)
        {
            var filepath = Bl.GenerateOpsti(opstiId);
            return filepath;
        }

        [WebMethod]
        public static string GeneratePosebniUslovi(int posebniId)
        {
            var filepath = Bl.GeneratePosebni(posebniId);
            return filepath;
        }
    }
}