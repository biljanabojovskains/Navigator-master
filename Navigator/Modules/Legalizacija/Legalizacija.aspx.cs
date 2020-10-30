using System;
using System.Web.Services;
using System.Web.UI;
using Navigator.Bll;
using Newtonsoft.Json;
using System.Configuration;
using System.Linq;
using System.IO;
using System.Web.Hosting;

namespace Navigator.Modules.Legalizacija
{
    public partial class Legalizacija : System.Web.UI.Page
    {
        protected override void OnPreLoad(EventArgs e)
        {
            if (!IsPostBack)
            {

                var modules = ConfigurationManager.AppSettings["mods"].Split(',');
                var modul = modules.Contains("m1lN1");
                if (!modul)
                {
                    Response.Redirect("~/NoAccess");
                }
            }
            base.OnPreLoad(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            FillMunicipalities();
            FillNamena();
        }
        private void FillMunicipalities()
        {
            var opstina = Bl.GetAllMunicipalities();
            selectOpstina.DataSource = opstina;
            selectOpstina.DataTextField = "ImeTema";
            selectOpstina.DataValueField = "ImeTema";
            selectOpstina.DataBind();
        }
        private void FillNamena()
        {
            var namena = Bl.GetNamena();
            namenaNaObjekt.DataSource = namena;
            namenaNaObjekt.DataTextField = "ImeTema";
            namenaNaObjekt.DataValueField = "ImeTema";
            namenaNaObjekt.DataBind();
        }
       
        [WebMethod]
        public static string SearchOutKat(string searchString)
        {
            var item = Bl.SearchKatParceliLegalizacija(searchString);
            var json = JsonConvert.SerializeObject(item);
            return json;
        }
        [WebMethod]
        public static string SearchOutPredmet(string searchString)
        {
            var item = Bl.SearchBrPredmetLegalizacija(searchString);
            var json = JsonConvert.SerializeObject(item);
            return json;
        }

      

        [WebMethod]
        public static int ZacuvajBaranje(string katastarskaOpstina, string katastarskaParcela, string broj, string namenaobjekt, string brojObjekt, string tipLegalizacija, string polygon)
        {
            int? brObjekt = null;
            string tip = null;
            if (brojObjekt == "")
            {
                brojObjekt = null;
            }
            else
            {
                brObjekt = Int32.Parse(brojObjekt);
            }

            if (tip == "")
            {
                tip = null;
            }
            else
            {
                tip = tipLegalizacija;
            }

            int item = Bl.InsertLegalizacija(katastarskaOpstina, katastarskaParcela, broj, namenaobjekt, tipLegalizacija, polygon, brObjekt);

            return item;

        }

        [WebMethod]
        public static string InfoLegalizacijaTool(string coordinates)
        {
            var item = Bl.GetInfoLegalizacija(coordinates);
            var result = JsonConvert.SerializeObject(item);
            return result;
        }

        [WebMethod]
        public static string CountLegalizacija()
        {
            var item = Bl.Count();
            var result = JsonConvert.SerializeObject(item);
            return result;
        }

        [WebMethod]
        public static bool UpdateStatusGradba(string id)
        {
            var item = Bl.UpdateStatusGradba(Int32.Parse(id));

            return item;
        }
        [WebMethod]
        public static void Izbrisi(string id)
        {
            var item = Bl.DeleteObjekt(Int32.Parse(id));
        }

        

          [WebMethod]
        public static bool ZacuvajDokumenti(string fileBase64, string filename, int id)
        {

            string path = HostingEnvironment.MapPath("~/File/");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string extension = Path.GetExtension(filename);

            string name = Guid.NewGuid() + extension;
            string pathDB = "File/" + name;

            fileBase64 = fileBase64.Substring(fileBase64.IndexOf(",") + 1);

            File.WriteAllBytes(path + name, Convert.FromBase64String(fileBase64));

            var item = Bl.InsertDoc(pathDB, filename, id);

            return item;
         
        }

          [WebMethod]
          public static string ListDokumenti(string id)
          {
              var item = Bl.ListDokumenti(Int32.Parse(id));
              var result = JsonConvert.SerializeObject(item);
              return result;
          }

          [WebMethod]
          public static void IzbrisiDokument(int id)
          {
              var item = Bl.DeleteDocument(id);
          }


    }
}