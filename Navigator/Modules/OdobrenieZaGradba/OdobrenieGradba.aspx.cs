using System;
using System.Web.Services;
using System.Web.UI;
using Navigator.Bll;
using Newtonsoft.Json;
using System.Globalization;
using System.Web;
using System.Configuration;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace Navigator.Modules.OdobrenieZaGradba
{
    public partial class OdobrenieGradba : Page
    {
      
        protected override void OnPreLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                var modules = ConfigurationManager.AppSettings["mods"].Split(',');
                var modul = modules.Contains("o1pN1");
                if (!modul)
                {
                    Response.Redirect("~/NoAccess");
                }
            }
            base.OnPreLoad(e);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            GetKatnaGaraza();
            GetTipBaranja();
        }
        private void GetKatnaGaraza()
        {
            var garazi = Bl.GetAllKatniGarazi();
            DdlGarazi.DataSource = garazi;
            DdlGarazi.DataBind();
            DdlGarazi.Items.Insert(0, "");
        }
        private void GetTipBaranja()
        {
            var baranja = Bl.GetAllTipBaranja();
            ddlTipBaranje.DataSource = baranja;
            ddlTipBaranje.DataBind();
            ddlTipBaranje.Items.Insert(0, "");
        }
        [WebMethod]
        public static string InfoTool(string coordinates)
        {
            var item = Bl.GetInfo(coordinates);
            string output = JsonConvert.SerializeObject(item);
            return output;
        }
        [WebMethod]
        public static string InfoOdobrenieTool(string coordinates)
        {
            var item = Bl.GetInfoOdobrenija(coordinates);
            var result = JsonConvert.SerializeObject(item);
            return result;
        }
        [WebMethod]
        public static string InfoDocTool(string coordinates)
        {
            var item = Bl.GetInfoDoc(coordinates);
            var result = JsonConvert.SerializeObject(item);
            return result;
        }
        [WebMethod]
        public static string SearchOut(string searchString)
        {
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
        public static void IzbrisiPredmet(int id)
        {
            var item = Bl.DeletePredmet(id);
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
        [WebMethod]
        public static bool Save(string fkParcela, string brPredmet, string tipBaranje, string sluzbenik, string datumBaranja, string datumIzdavanja, string datumPravosilno, string investitor, string brKP, string ko, string adresa, string parkingMestaParcela, string parkingMestaGaraza, string katnaGaraza, string iznosKomunalii, string zabeleski, string podtipBaranje)
        {
            string path = "";


            if (podtipBaranje == null)
            {
                var item = Bl.InsertOdobrenie(Int32.Parse(fkParcela), brPredmet, tipBaranje, sluzbenik, DateTime.Parse(datumBaranja), DateTime.Parse(datumIzdavanja), DateTime.Parse(datumPravosilno), investitor, brKP, ko, adresa, parkingMestaParcela, parkingMestaGaraza, katnaGaraza, Int32.Parse(iznosKomunalii), zabeleski, path);
                return item;
            }

            else
            {
                var item = Bl.InsertOdobrenieSoPodtip(Int32.Parse(fkParcela), brPredmet, tipBaranje, sluzbenik, DateTime.Parse(datumBaranja), DateTime.Parse(datumIzdavanja), DateTime.Parse(datumPravosilno), investitor, brKP, ko, adresa, parkingMestaParcela, parkingMestaGaraza, katnaGaraza, Int32.Parse(iznosKomunalii), zabeleski, path, podtipBaranje);
                return item;
            }

        }
        protected void btnZacuvaj_Click(object sender, EventArgs e)
        {
            var dateBaranja = DateTime.ParseExact(datumBaranja.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var dateIzdavanja = DateTime.ParseExact(datumIzdavanja.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var datePravosilno = DateTime.ParseExact(pravosilno.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            string id = fkParcel.Value;
            int fkParcela = Convert.ToInt32(id);
            string komunalii = iznosKomunalni.Value;
            //int iznosKomunalii = Convert.ToInt32(komunalii);
            double iznosKomunalii = Double.Parse(komunalii, CultureInfo.InvariantCulture);
            string selectedKO = ddlKO.SelectedItem.Text;
            string pateka = "";
            string podtipBaranje = fkPodtipBaranje.Value;
            if (podtipBaranje == null)
            {
                var insert = Bl.InsertOdobrenie(fkParcela, brPredmet.Value, ddlTipBaranje.SelectedItem.Text, sluzbenik.Value, dateBaranja.Date, dateIzdavanja.Date, datePravosilno.Date, investitor.Value, brKP.Value, selectedKO.ToString(), adresa.Value, parkMestoPacela.Value, parkMestoGaraza.Value, DdlGarazi.SelectedItem.Text, iznosKomunalii, zabeleska.Value, pateka);
            }
            else
            {
                var insert = Bl.InsertOdobrenieSoPodtip(fkParcela, brPredmet.Value, ddlTipBaranje.SelectedItem.Text, sluzbenik.Value, dateBaranja.Date, dateIzdavanja.Date, datePravosilno.Date, investitor.Value, brKP.Value, selectedKO.ToString(), adresa.Value, parkMestoPacela.Value, parkMestoGaraza.Value, DdlGarazi.SelectedItem.Text, iznosKomunalii, zabeleska.Value, pateka, podtipBaranje);
            }



            fkParcel.Value = "";
            brPredmet.Value = "";
            sluzbenik.Value = "";
            datumBaranja.Text = "";
            datumIzdavanja.Text = "";
            pravosilno.Text = "";
            investitor.Value = "";
            brKP.Value = "";
            fkPodtipBaranje.Value = "";
            adresa.Value = "";
            parkMestoPacela.Value = "";
            parkMestoGaraza.Value = "";
            iznosKomunalni.Value = "";
            zabeleska.Value = "";
            ddlKO.ClearSelection();
            DdlGarazi.ClearSelection();
            ddlTipBaranje.ClearSelection();
        }
        protected void btnDocInsert_Click(object sender, EventArgs e)
        {
            string SaveLocation = "";
            string pathDB = "";
            if ((File1.PostedFile != null) && (File1.PostedFile.ContentLength > 0))
            {
                //string fn = System.IO.Path.GetFileName(File1.PostedFile.FileName);

                string ext = System.IO.Path.GetExtension(File1.PostedFile.FileName);
                string name = Guid.NewGuid() + ext;
                if (!Directory.Exists(Server.MapPath("~/Data/")))
                    Directory.CreateDirectory(Server.MapPath("~/Data/"));
                SaveLocation = Server.MapPath("~/Data/") + name;
                pathDB = "Data/" + name;
                try
                {
                    File1.PostedFile.SaveAs(SaveLocation);

                    string id = fkDocParcel.Value;
                    int fkParcela = Convert.ToInt32(id);
                    var insert = Bl.InsertDoc(fkParcela, pathDB);


                    fkDocParcel.Value = "";
                    pathDB = "";

                }
                catch (Exception ex)
                {
                    Response.Write("Error: " + ex.Message);
                    //Note: Exception.Message returns a detailed message that describes the current exception. 
                    //For security reasons, we do not recommend that you return Exception.Message to end users in 
                    //production environments. It would be better to put a generic error message. 
                }
            }
            else
            {
                //Response.Write("Please select a file to upload.");
            }
        }

        [WebMethod]
        public static string FillSubRequest(int id)
        {
            var item = Bl.GetAllPodTipBaranja();
            var result = JsonConvert.SerializeObject(item);
            return result;
        }

        [WebMethod]
        public static string GenerateNumerickiPokazateli(int numerickiId)
        {
            var filepath = Bl.GenerateNumericki(numerickiId);
            return filepath;
        }

        [WebMethod]
        public static string GenerateTehnickiIspravki(int tehnickiIspravkiId)
        {
            var filepath = Bl.GenerateTehnickiIspravki(tehnickiIspravkiId);
            return filepath;
        }
    }
}